using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudPortal : MudEntity
    {
        public List<MudPortalEntry> Entries {get;}
        public MudPortal(int id, string name) : base(id, name)
        {
            Entries = new List<MudPortalEntry>();
        }

        public void AddEntry(MudPortalEntry entry)
        {
            if (!Entries.Any(x => x.Direction == entry.Direction))
                return;
            Entries.Add(entry);
        }

        public bool HasEntriesWithRoom(int roomId)
        {
            return Entries.Any(x => x.StartRoom == roomId || x.EndRoom == roomId);
        }

        // public bool IsValidPortal(string direction)
        // {
        //     return Entries.Any(x => x.Direction == direction && x.StartRoom == Room);
        // }
    }

}