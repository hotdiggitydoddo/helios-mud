using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Containers
{
    public class ComponentSet
    {
        private List<MudComponent> _components;

        public int EntityId { get; }
        public int Count => _components.Count;
        

        public ComponentSet(int entity)
        {
            EntityId = entity;
            _components = new List<MudComponent>();
        }
         
        public bool Has(string name)
        {
            return _components.Any(x => x.Name == name);
        }

        public MudComponent Add(MudComponent component)
        {
            if (Has(component.Name)) return null;
           
            _components.Add(component);
            //TODO: listeners notify that trait was added
            return component;
        }

        public MudComponent Get(string name)
        {
            return _components.SingleOrDefault(x => x.Name == name);
        }

        public void Remove(string name)
        {
            var existing = _components.SingleOrDefault(x => x.Name == name);
            //TODO notify listners of trait removal
            _components.Remove(existing);
        }

        public MudComponent[] GetAll()
        {
            return _components.ToArray();
        }

        public bool DoAction(MudAction action)
        {
            foreach (var c in _components)
            {
                if (c.IsActive && !c.DoAction(action))
                    return false;
            }
            return true;
        }
    }
}