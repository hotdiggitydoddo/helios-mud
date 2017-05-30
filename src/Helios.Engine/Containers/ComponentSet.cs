using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Containers
{
    public class ComponentSet
    {
        private List<MudComponent> _components;
        private List<TimedMudAction> _actionHooks;

        public int EntityId { get; }
        public int Count => _components.Count;


        public ComponentSet(int entity)
        {
            EntityId = entity;
            _components = new List<MudComponent>();
            _actionHooks = new List<TimedMudAction>();
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

        //Timed logic

        public void AddHook(TimedMudAction hook)
        {
            _actionHooks.Add(hook);
        }

        public void RemoveHook(TimedMudAction hook)
        {
            _actionHooks.Remove(hook);
        }

        public void ClearHooks()
        {
            for (int i = _actionHooks.Count - 1; i >= 0; i--)
            {
                _actionHooks[i].Unhook();
                _actionHooks.Remove(_actionHooks[i]);
            }
        }

        public void ClearComponentHooks(string componentName)
        {
            for (int i = _actionHooks.Count - 1; i >= 0; i--)
            {
                if (_actionHooks[i].Type == "messagecomponent" || _actionHooks[i].Type == "delcomponent")
                {
                    if (_actionHooks[i].Args[0] == componentName)
                    {
                        _actionHooks[i].Unhook();
                        _actionHooks.Remove(_actionHooks[i]);
                    }
                }
            }
        }

        public void KillHook(string actionType, string componentName)
        {
            for (int i = _actionHooks.Count - 1; i >= 0; i--)
            {
                if (_actionHooks[i].Type == actionType && _actionHooks[i].Args[0] == componentName)
                {
                    _actionHooks[i].Unhook();
                    _actionHooks.Remove(_actionHooks[i]);
                }
            }
        }
    }
}