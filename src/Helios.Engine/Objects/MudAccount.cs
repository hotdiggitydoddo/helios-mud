using System.Collections.Generic;

namespace Helios.Engine.Objects
{
    public class MudAccount
    {
        public int Id { get; }
        public int UserId { get; }
        public List<MudEntity> Characters {get; private set;}
        public MudEntity CurrentCharacter {get; private set;}

        public MudAccount(int id, int userId)
        {
            Id = id;
            UserId = userId;
            Characters = new List<MudEntity>();
        }
    }
}