using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Locations
{
    public class MudWorld : MudEntity
    {
        private readonly Dictionary<int, MudZone> _zones;
        private readonly Dictionary<int, MudRoom> _rooms;
        private Dictionary<int, MudEntity> _entities;

        public MudWorld(int id, string name = null) : base(id, name)
        {
            _zones = new Dictionary<int, MudZone>();
            _rooms = new Dictionary<int, MudRoom>();
            _entities = new Dictionary<int, MudEntity>();
        }

        public MudEntity GetRoomById(int roomId)
        {
            return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
        }

        public MudEntity GetZoneById(int zoneId)
        {
            return _zones.ContainsKey(zoneId) ? _zones[zoneId] : null;
        }

        public MudEntity GetEntityWithId(int id)
        {
            return _entities.ContainsKey(id) ? _entities[id] : null;
        }

       
        public List<MudEntity> FindEntities(params int[] ids)
        {
            return  _entities.Values.Where(x => ids.Contains(x.Id)).ToList();
        }
    }
}