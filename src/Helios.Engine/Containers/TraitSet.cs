using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Objects;

namespace Helios.Engine.Containers
{
    public class TraitSet
    {
        private List<MudTrait> _traits;

        public int EntityId { get; }
        public int Count => _traits.Count;
        

        public TraitSet(int entity)
        {
            EntityId = entity;
            _traits = new List<MudTrait>();
        }
         
        public bool Has(string name)
        {
            return _traits.Any(x => x.Name == name);
        }

        public MudTrait Add(string name, string val)
        {
            if (Has(name)) return null;

            var trait = new MudTrait(name, val);
            _traits.Add(trait);
            //TODO: listeners notify that trait was added
            return trait;
        }

        public MudTrait Get(string name)
        {
            return _traits.SingleOrDefault(x => x.Name == name);
        }

        public string Change(string name, string newVal)
        {
            var existing = _traits.SingleOrDefault(x => x.Name == name);
            var oldVal = existing.Value;
            existing.Value = newVal;
            //TODO notify listeners of trait change
            return oldVal;
        }

        public MudTrait Set(string name, string val)
        {
            var trait = _traits.SingleOrDefault(x => x.Name == name);
            if (trait != null)
                 trait.Value = val;
            else
                trait = new MudTrait(name, val);
            return trait;
        }

        public void Remove(string name)
        {
            var existing = _traits.SingleOrDefault(x => x.Name == name);
            //TODO notify listners of trait removal
            _traits.Remove(existing);
        }

        public MudTrait[] GetAll()
        {
            return _traits.ToArray();
        }
    }
}