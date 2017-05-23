using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Helios.Engine.Connections;
using Helios.Engine.Containers;
using Helios.Engine.Objects;
using Helios.Engine.Scripting;
using MoonSharp.Interpreter;

namespace Helios.Engine.Factories
{
    public class ComponentManager
    {
        private static readonly ComponentManager _instance = new ComponentManager();
        public static ComponentManager Instance => _instance;

        private Dictionary<string, string> _components;

        public ComponentManager()
        {
            _components = new Dictionary<string, string>();
        }

        public void RefreshAllComponents()
        {
            _components.Clear();
            _components = ScriptManager.Instance.GetComponentScripts();
        }

        public void AssignComponent(MudEntity entity, string componentName, params MudTrait[] defaults)
        {
            var script = new Script();
            script.Globals["MudComponent"] = typeof(MudComponent);
            script.Globals["MudTrait"] = typeof(MudTrait);
            script.Globals["TraitSet"] = typeof(TraitSet);
            script.Globals["MudEntity"] = typeof(MudEntity);
            script.DoString(_components[componentName]);

            var args = new Dictionary<string, string>();
            foreach (var trait in defaults)
                args.Add(trait.Name, trait.Value);

            var cmp = (MudComponent)script.Call(script.Globals["init"], entity, script, args).UserData.Object;
            entity.Components.Add(cmp);
        }
    }
}