using System;
using System.Collections.Generic;
using System.Text;

namespace Helios.Domain.Models
{
    public class Trait : EntityBase
    {
        public int EntityId {get; set;}
        public string Name {get; set;}
        public string Value {get; set;}

        //navigation prop
        public Entity Entity {get; set;}
    }
}