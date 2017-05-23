using System;
using System.Collections.Generic;
using System.Text;

namespace Helios.Domain.Models
{
    public class Entity : EntityBase
    {
        public string Name {get; set;}
        public List<Trait> Traits {get; set;}
        public List<Entity_Component> Components {get; set;}
    }
}
