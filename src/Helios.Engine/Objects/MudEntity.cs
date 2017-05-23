using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Containers;

namespace Helios.Engine.Objects
{
    public class MudEntity
    {
        public int Id { get; }
        public string Name { get; set; }
        public TraitSet Traits { get; }
        public ComponentSet Components { get; }

        public MudEntity(int id, string name = null)
        {
            Traits = new TraitSet(id);
            Components = new ComponentSet(id);
            Name = name ?? "-none-";
            Id = id;
        }

        public bool DoAction(MudAction action)
        {
            return Components.DoAction(action);
        }
    }
}