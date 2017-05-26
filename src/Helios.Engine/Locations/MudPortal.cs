using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudPortal : MudEntity
    {
        public int Room {get;}
        public List<MudPortalEntry> Entries {get;}
        public MudPortal(int id, int room, string name) : base(id, name)
        {
            Entries = new List<MudPortalEntry>();
            Room = room;
        }

        public void AddEntry(MudPortalEntry entry)
        {
            if (!Entries.Any(x => x.Direction == entry.Direction))
                return;
            Entries.Add(entry);
        }

        public bool IsValidPortal(string direction)
        {
            return Entries.Any(x => x.Direction == direction && x.StartRoom == Room);
        }
    }

}