using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Containers
{
    public class CommandSet
    {
        private List<MudCommand> _commands;

        public int EntityId { get; }

        public CommandSet(int entity)
        {
            EntityId = entity;
            _commands = new List<MudCommand>();
        }
         
        public bool Has(string name)
        {
            return _commands.Any(x => x.Name == name);
        }

        public MudCommand Add(MudCommand component)
        {
            if (Has(component.Name)) return null;
           
            _commands.Add(component);
            //TODO: listeners notify that trait was added
            return component;
        }

        public MudCommand Get(string name)
        {
            return _commands.SingleOrDefault(x => x.Name == name);
        }

        public void Remove(string name)
        {
            var existing = _commands.SingleOrDefault(x => x.Name == name);
            //TODO notify listners of trait removal
            _commands.Remove(existing);
        }

        public MudCommand[] GetAll()
        {
            return _commands.ToArray();
        }
    }
}