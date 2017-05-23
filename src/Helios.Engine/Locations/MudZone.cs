using System.Collections.Generic;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudZone : MudEntity
    {
        public List<int> Rooms {get; private set;}
        public MudZone(int id, string name = null) : base(id, name)
        {
            Rooms = new List<int>();
        }
    }
}