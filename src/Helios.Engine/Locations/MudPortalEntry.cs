using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudPortalEntry : MudEntity
    {
        public MudPortalEntry(int id, int startRoom, int endRoom, string direction, string name = null) : base(id, name)
        {
            StartRoom = startRoom;
            EndRoom = endRoom;
            Direction = direction;
        }

        public int StartRoom {get;}             // starting room
        public int EndRoom {get;}      // ending room
        public string Direction {get;}      // name of the direction used to enter portal
    }

}