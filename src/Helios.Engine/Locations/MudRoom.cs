using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudRoom : MudEntity
    {
        public int Zone {get; private set;}
        //public List<int> Mobs => Entities.W
        //public List<int> Items {get; private set;}
        public List<int> Portals {get; private set;}
        public List<int> Entities {get; private set;}
       // private List<Mob> _mobs; 
       // private List<Item> _items; 
    
        public MudRoom(int id, int zone, string name) : base(id, name)
        {
            //Mobs = new List<int>();
            //Items = new List<int>();
            Entities = new List<int>();
            Portals = new List<int>();
            Zone = zone;
        }
    }
}