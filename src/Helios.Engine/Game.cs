using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Objects;
using Helios.Engine.Connections;
using Helios.Engine.UI;
using Helios.Engine.Scripting;
using Helios.Engine.Factories;
using Helios.Engine.Locations;
using System.Linq.Expressions;

namespace Helios.Engine
{
    public class Game
    {
        private IOutputFormatter _formatter;
        private IMessageHandler _msgHandler;
        private Dictionary<string, ActionRunner> _actionRunners;
        private Dictionary<int, MudEntity> _entities;
        private Dictionary<int, MudRoom> _rooms;
        private Dictionary<int, MudZone> _zones;
        private Dictionary<int, MudPortal> _portals;
        private Dictionary<int, MudPortalEntry> _portalEntries;
        private IEntityFactory _entityFactory;
        private long _lastTime;
        private Timer _timer;
        private bool _updating;
        public static Game Instance;
        public long TimeRunning { get; private set; }
        public long CurrentTime { get; private set; }

        public Game(IEntityFactory entityFactory, IOutputFormatter formatter)
        {
            Instance = this;
            _entities = new Dictionary<int, MudEntity>();
            _rooms = new Dictionary<int, MudRoom>();
            _zones = new Dictionary<int, MudZone>();
            _portals = new Dictionary<int, MudPortal>();
            _portalEntries = new Dictionary<int, MudPortalEntry>();


            _entityFactory = entityFactory;
            _formatter = formatter;

            ScriptManager.Instance.RefreshScripts(ScriptType.Game);
            ScriptManager.Instance.RefreshScripts(ScriptType.MudComponent);
            ComponentManager.Instance.RefreshAllComponents();
        }

        public void Init(IMessageHandler handler)
        {
            _msgHandler = handler;
            SetupWorld();
        }

        void SetupWorld()
        {
            var zone = new MudZone(1, "The Enchanted Forest");
            
            var room1 = new MudRoom(1, 1, "Western Clearing");
            var room2 = new MudRoom(2, 1, "Eastern Thicket");
            var room3 = new MudRoom(3, 1, "Camp");

            //camp --> west
            var p1 = new MudPortal(1, 3, "Passageway 1");
            var p1e1 = new MudPortalEntry(1, room3.Id, room1.Id, "West");
            var p1e2 = new MudPortalEntry(2, room1.Id, room3.Id, "East");
            p1.Entries.AddRange(new [] { p1e1.Id, p1e2.Id });

            //camp --> east
            var p2 = new MudPortal(2, 3, "Passageway 2");
            var p2e1 = new MudPortalEntry(3, room3.Id, room2.Id, "East");
            var p2e2 = new MudPortalEntry(4, room2.Id, room3.Id, "West");
            p2.Entries.AddRange(new [] { p1e1.Id, p1e2.Id });

            room3.Portals.Add(p1.Id);
            room3.Portals.Add(p2.Id);

            room1.Portals.Add(p1.Id);

            room2.Portals.Add(p2.Id);

            zone.Rooms.AddRange(new []{ room1.Id, room2.Id, room3.Id });

            _zones.Add(zone.Id, zone);
            _rooms.Add(room1.Id, room1);
            _rooms.Add(room2.Id, room2);
            _rooms.Add(room3.Id, room3);
            _portals.Add(p1.Id, p1);
            _portals.Add(p2.Id, p2);
            _portalEntries.Add(p1e1.Id, p1e1);
            _portalEntries.Add(p1e2.Id, p1e2);
            _portalEntries.Add(p2e1.Id, p2e1);
            _portalEntries.Add(p2e2.Id, p2e2);

        }

        public void Start()
        {
            TimeRunning = 0;
            _lastTime = DateTime.UtcNow.Ticks;
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1000 / 60));
        }

        public void DoAction(MudAction action)
        {
            var type = action.Type.ToLower();

            if (type == "enterworld")
                Login(action);
            else if (type == "leaveworld")
                Logout(action);
            // else if (type == "attemptgetitem")
            //     GetItem(action.SenderId, action.ReceiverId, action.OtherEntity1);

            //custom action
            else if (_actionRunners.ContainsKey(type))
                _actionRunners[type].Run(action);
        }

        public MudEntity CreatePlayerCharacter(int accountId, MudEntity model)
        {
            return _entityFactory.CreatePlayerCharacter(accountId, model);
        }

        public List<MudEntity> LoadPlayerCharacters(int accountId)
        {
            return _entityFactory.LoadCharactersFromAccount(accountId);
        }

        public Dictionary<int, MudEntity> GetEntitiesWithTrait(string traitName)
        {
            return (Dictionary<int, MudEntity>)_entities.Where(e => e.Value.Traits.Has(traitName));
        }

        public MudEntity GetEntityById(int id)
        {
            return _entities.ContainsKey(id) ? _entities[id] : null;
        }
        // public MudEntity GetRoomById(int roomId)
        // {
        //     return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
        // }

        // public MudEntity GetZoneById(int zoneId)
        // {
        //     return _zones.ContainsKey(zoneId) ? _zones[zoneId] : null;
        // }

        public void SendMessage(int accountId, string message, params string[] args)
        {
            _msgHandler.SendToAccount(accountId, _formatter.Write(message), args);
        }

        public void SendToAll(string message, params string[] args)
        {
            _msgHandler.SendToAll(_formatter.Write(message), args);
        }

        public List<MudEntity> GetEntitiesInRoom(int roomId, Expression<Func<MudEntity, bool>> predicate = null)
        {
            var room = _rooms[roomId];
            var query = _entities.Values.AsQueryable();
            query = query.Where(x => room.Entities.Contains(x.Id));
            if (predicate != null)
                query = query.Where(predicate);
            return query.ToList();
        }

        public MudRoom GetRoomWithEntity(int entityId)
        {
            var mob = _entityFactory.GetEntityById(entityId);
            if (mob == null) return null;
            var roomTrait = mob.Traits.Get("room")?.Value;
            var roomId = !string.IsNullOrWhiteSpace(roomTrait) ? int.Parse(roomTrait) : 0;

            return roomId > 0 ? _rooms[roomId] : null;
        }

        public List<MudEntity> GetAllPlayers()
        {
            return _entities.Values.Where(x => x.Traits.Has("accountId")).ToList();
        }


        private void Tick(long elapsedTime)
        {

        }

        private void OnTimerElapsed(object state)
        {
            if (_updating) return;
            _updating = true;

            CurrentTime = DateTime.UtcNow.Ticks;

            var elapsed = (CurrentTime - _lastTime);

            Tick(elapsed);

            TimeRunning += elapsed;
            _lastTime = CurrentTime;

            _updating = false;
        }

        private void Login(MudAction action)
        {
            var character = _entityFactory.GetEntityById(action.SenderId);
            if (character == null) return;
            if (!_entities.ContainsKey(character.Id))
                _entities.Add(character.Id, character);
            
            character.Components.Add(new ReporterComponent(character, "reporter", null));

            var roomTrait = character.Traits.Get("room")?.Value;
            var roomId = !string.IsNullOrWhiteSpace(roomTrait) ? int.Parse(roomTrait) : 0;
            if (roomId > 0)
            {
                var room = _rooms[roomId];
                room.Entities.Add(character.Id);

                ActionRealmPlayers(new MudAction("enterrealm", character.Id));
                
                var zone = _zones[room.Zone];
                var enterZone = new MudAction("enterzone", character.Id);
                var enterRoom = new MudAction("enterroom", character.Id, 0);
                
                zone.DoAction(enterRoom);
                character.DoAction(enterZone);
                room.DoAction(enterRoom);

                ActionRoomMobs(enterRoom, room.Id);
                ActionRoomItems(enterRoom, room.Id);
            }
        }

        private void Logout(MudAction action)
        {
             var character = _entityFactory.GetEntityById(action.SenderId);
             var room = GetRoomWithEntity(character.Id);
             var zone = _zones[room.Zone];

             //tell everyone about it
             var leaveRoom = new MudAction("leaveroom", character.Id, 0);
             ActionRoomItems(leaveRoom, room.Id);
             ActionRoomMobs(leaveRoom, room.Id);
             room.DoAction(leaveRoom);

             var leaveZone = new MudAction("leavezone", character.Id);
             character.DoAction(leaveZone);
             zone.DoAction(leaveZone);
             ActionRealmPlayers(new MudAction("leaverealm", character.Id));

             //remove from game
             room.Entities.Remove(character.Id);
             _entities.Remove(character.Id);
        }


        private void EnterPortal(MudAction action)
        {
            var mob = _entities[action.SenderId];
            var portal = _portals[action.ReceiverId];
            int oldRoomId;

            if (!int.TryParse(mob.Traits.Get("room")?.Value, out oldRoomId))
                return;

            var oldRoom = _rooms[oldRoomId];

            if (!oldRoom.Portals.Contains(portal.Id))
                // log - mob cannot enter a portab when it's not in the room they're in
                return;

            // get the destination room
            var entry = _portalEntries.Values.Where(x => x.StartRoom == oldRoomId).Single(x => x.Direction == action.Args[0]);
            var newRoom = _rooms[entry.EndRoom];
            var changeZone = oldRoom.Zone != newRoom.Zone;
            var oldZone = _zones[oldRoom.Zone];
            var newZone = _zones[newRoom.Zone];

            // ask permission of everyone to leave current room
            if (changeZone)
            {
                var canLeaveZone = new MudAction("canleavezone", mob.Id, oldZone.Id);
                var canEnterZone = new MudAction("canenterzone", mob.Id, newZone.Id);
                if (!oldZone.DoAction(canLeaveZone))
                    return;
                if (!newZone.DoAction(canEnterZone))
                    return;
                if (!mob.DoAction(canLeaveZone))
                    return;
                if (!mob.DoAction(canEnterZone))
                    return;
            }

            var canLeaveRoom = new MudAction("canleaveroom", mob.Id);
            var canEnterRoom = new MudAction("canenterroom", mob.Id);
            var canEnterPortal = new MudAction("canenterportal", mob.Id);

            if (!oldRoom.DoAction(canLeaveRoom))
                return;
            if (!newRoom.DoAction(canEnterRoom))
                return;
            if (!mob.DoAction(canLeaveRoom))
                return;
            if (!portal.DoAction(canEnterPortal))
                return;

            //tell the room/region that the player is leaving
            if (changeZone)
            {
                var leaveZone = new MudAction("leavezone", mob.Id);
                oldZone.DoAction(leaveZone);
                mob.DoAction(leaveZone);
            }

            var leaveRoom = new MudAction("leaveroom", mob.Id);
            ActionRoomMobs(leaveRoom, oldRoom.Id);
            ActionRoomItems(leaveRoom, oldRoom.Id);

            //tell the portal the mob has entered
            var enterPortal = new MudAction("enterportal", mob.Id, portal.Id);
            portal.DoAction(enterPortal);
            mob.DoAction(enterPortal);

            //now move the mob
            oldRoom.Entities.Remove(mob.Id);
            mob.Traits.Change("room", newRoom.Id.ToString());
            newRoom.Entities.Add(mob.Id);

            //tell everyone in the region/room that the player has entered
            if (changeZone)
            {
                var enterZone = new MudAction("enterzone", mob.Id, newZone.Id);
                newZone.DoAction(enterZone);
                mob.DoAction(enterZone);
            }

            var enterRoom = new MudAction("enterroom", mob.Id, portal.Id);
            newRoom.DoAction(enterRoom);
            ActionRoomMobs(enterRoom, newRoom.Id);
            ActionRoomItems(enterRoom, newRoom.Id);
        }

        private void Transfer(MudAction action)
        {
            /*
                mob to room
                item to room
                mob to mob 
             */

            var requestor = _entities[action.SenderId];
            var receiver = _entities[action.ReceiverId];
            var subject = _entities[action.OtherEntity1];

            var canMove = new MudAction("canMove", requestor.Id, receiver.Id, subject.Id);

            // 1) if there is another entity involved, as permission from it
            if (subject != null && !subject.DoAction(canMove))
                return;
            // 2) ask permission from requestor and parent, if any
            if (!requestor.DoAction(canMove))
                return;
            // 3) ask permission from receiver and its parent, if any
            if (!receiver.DoAction(canMove))
                return;

            // 4) move the entity to the recipient






        }

        private void ActionRoomMobs(MudAction action, int roomId)
        {
            var room = _rooms[roomId];
            if (room == null) return;

            var mobs = GetEntitiesInRoom(roomId, x => x.Traits.Has("race"));

            foreach (var mob in mobs)
                mob.DoAction(action);
        }

        private void ActionRoomItems(MudAction action, int roomId)
        {
            var room = _rooms[roomId];
            if (room == null) return;

            var mobs = GetEntitiesInRoom(roomId, x => x.Traits.Has("item"));

            foreach (var mob in mobs)
                mob.DoAction(action);
        }

        private void ActionRealmPlayers(MudAction action)
        {
            var players = GetAllPlayers();
            foreach(var player in players)
                player.DoAction(action);
        }


        // private void Transfer(MudAction action)
        // {
        //     var transferringEntity = _entities[action.SenderId];
        //     MudEntity acceptingEntity;
        //     MudEntity optionalEntity = null;
        //     MudEntity parentEntity = null;

        //     try
        //     {
        //         // retrieve entities in question
        //         if (action.Type == "attemptenterportal")
        //         {
        //             acceptingEntity = _portals[action.ReceiverId];
        //             parentEntity = _rooms[int.Parse(transferringEntity.Traits.Get("room").Value)];
        //         }
        //         else if (action.Type == "attemptgiveitem")
        //         {
        //             acceptingEntity = _entities[action.ReceiverId];
        //             optionalEntity = _entities[action.OtherEntity1];
        //         }
        //         else if (action.Type == "attemptdropitem")
        //         {
        //             acceptingEntity = _rooms[action.ReceiverId];
        //             optionalEntity = _entities[action.OtherEntity1];
        //         }
        //     }
        //     catch
        //     {
        //         return;
        //     }
        //     if (action.Type == "attemptenterportal" && parentEntity)
        //     {

        //     }


        //     //integrity check
        //     //ask for permission
        //     //physical movement
        //     //notify everyone
        //     //clean up
        // }
    }


}