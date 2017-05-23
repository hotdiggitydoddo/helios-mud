using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudPortal : MudEntity
    {
        public int Room {get;}
        public List<int> Entries {get;}
        public MudPortal(int id, int room, string name) : base(id, name)
        {
            Entries = new List<int>();
            Room = room;
        }
    }

}