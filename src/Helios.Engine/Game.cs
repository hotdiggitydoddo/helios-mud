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
        private TimerRegistry _timerRegistry;
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
        public CommandManager Commands { get; }

        public Game(IEntityFactory entityFactory, IOutputFormatter formatter)
        {
            Instance = this;
            _formatter = formatter;
            _timerRegistry = new TimerRegistry();

            _entities = new Dictionary<int, MudEntity>();
            _rooms = new Dictionary<int, MudRoom>();
            _zones = new Dictionary<int, MudZone>();
            _portals = new Dictionary<int, MudPortal>();
            _portalEntries = new Dictionary<int, MudPortalEntry>();

            _entityFactory = entityFactory;

            ScriptManager.Instance.RefreshScripts(ScriptType.Game);
            ScriptManager.Instance.RefreshScripts(ScriptType.MudComponent);
            ScriptManager.Instance.RefreshScripts(ScriptType.MudCommand);
            ComponentManager.Instance.RefreshAllComponents();
            Commands = new CommandManager();
            Commands.LoadAllCommands();
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
            room1.Traits.Add("description", "The passageway gives way to an expanse of land unobstructed by trees of any shape or size.");
            var room2 = new MudRoom(2, 1, "Eastern Thicket");
            room2.Traits.Add("description", "You find yourself in the midst of a densely packed bushes and shrubs.");
            var room3 = new MudRoom(3, 1, "Camp");
            room3.Traits.Add("description", "Surrounded by thick shrubbery and large, looming pine trees lies a modest encampment.  At its center, a fire crackles softly.");

            //add an item
            var torch = new MudEntity(777, "torch");
            torch.Traits.Add("item", "true");
            torch.Traits.Add("quantity", "1");
            ComponentManager.Instance.AssignComponent(torch, "item");
            _entities.Add(torch.Id, torch);
            room3.Entities.Add(torch.Id);
            torch.Traits.Add("room", room3.Id.ToString());

            //add a mouse and snake and bear
            var mouse = new MudEntity(888, "mouse");
            mouse.Traits.Add("race", "mouse");
            _entities.Add(mouse.Id, mouse);
            room3.Entities.Add(mouse.Id);

            var snake = new MudEntity(999, "snake");
            snake.Traits.Add("race", "snake");
            _entities.Add(snake.Id, snake);
            room3.Entities.Add(snake.Id);

            var bear = new MudEntity(222, "brown bear");
            bear.Traits.Add("race", "bear");
            _entities.Add(bear.Id, bear);
            room3.Entities.Add(bear.Id);


            //camp --> west
            var p1 = new MudPortal(1001, "Passageway 1");
            var p1e1 = new MudPortalEntry(1, room3.Id, room1.Id, "west");
            var p1e2 = new MudPortalEntry(2, room1.Id, room3.Id, "east");
            p1.Entries.AddRange(new[] { p1e1, p1e2 });

            //camp --> east
            var p2 = new MudPortal(1002, "Passageway 2");
            var p2e1 = new MudPortalEntry(3, room3.Id, room2.Id, "east");
            var p2e2 = new MudPortalEntry(4, room2.Id, room3.Id, "west");
            p2.Entries.AddRange(new[] { p2e1, p2e2 });

            room3.Portals.Add(p1.Id);
            room3.Portals.Add(p2.Id);

            room1.Portals.Add(p1.Id);

            room2.Portals.Add(p2.Id);

            zone.Rooms.AddRange(new[] { room1.Id, room2.Id, room3.Id });

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
            else if (type == "infotoplayer")
                GameToPlayer(action);
            else if (type == "look")
                RouteActionToEntity(action.SenderId, action);
            else if (type == "listinventory")
                RouteActionToEntity(action.SenderId, action);
            else if (type == "attemptsay")
                Say(action);
            else if (type == "attemptenterportal")
                EnterPortal(action);
            else if (type == "attemptreceive")
                Transfer(action);

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

        public MudEntity GetEntityByName(string name)
        public MudEntity GetEntityById(int id)
        {
            return _entities.ContainsKey(id) ? _entities[id] : null;
        }

        public MudPortal GetPortalById(int id)
        {
            return _portals.ContainsKey(id) ? _portals[id] : null;
        }
        public MudRoom GetRoomById(int roomId)
        {
            return _rooms.ContainsKey(roomId) ? _rooms[roomId] : null;
        }

        public List<MudPortal> GetRoomPortals(int roomId)
        {
            return _portals.Values.Where(x => x.HasEntriesWithRoom(roomId)).ToList();
        }

        public MudZone GetZoneById(int zoneId)
        {
            return _zones.ContainsKey(zoneId) ? _zones[zoneId] : null;
        }

        public int FindEntityInRoom(string search, int roomId)
        {
            var entity = GetEntitiesInRoom(roomId, x => x.Name.StartsWith(search)).FirstOrDefault();
            return entity != null ? entity.Id : 0;
        }

        public void SendMessage(int accountId, string message, params string[] args)
        {
            _msgHandler.SendToAccount(accountId, _formatter.Write(message), args);
        }

        public void SendToAll(string message, params string[] args)
        {
            _msgHandler.SendToAll(_formatter.Write(message), args);
        }

        private void ExitToMainMenu(int accountId)
        {
            _msgHandler.Logout(accountId);
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
            var mob = _entities[entityId];
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
            _timerRegistry.Dispatch();
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

        private void Say(MudAction action)
        {
            //TODO: check if entity CAN talk
            var mob = _entities[action.SenderId];
            var say = new MudAction("say", mob.Id, 0, 0, action.Args[0], mob.Name);
            ActionRoomMobs(say, GetRoomWithEntity(mob.Id).Id);
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

                Commands.AssignCommand(character.Id, "quit");
                Commands.AssignCommand(character.Id, "look");
                Commands.AssignCommand(character.Id, "say");
                Commands.AssignCommand(character.Id, "west");
                Commands.AssignCommand(character.Id, "east");
                Commands.AssignCommand(character.Id, "take");
                Commands.AssignCommand(character.Id, "items");

                _timerRegistry.Add(new TimedMudAction(5, "infotoplayer", character.Id, "This is an action delayed by 5 seconds."));
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

            ExitToMainMenu(int.Parse(character.Traits.Get("accountId").Value));
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

        private void GameToPlayer(MudAction action)
        {
            var character = _entities[action.SenderId];
            var acctId = character.Traits.Get("accountId")?.Value;
            if (string.IsNullOrWhiteSpace("accountId"))
                return;
            SendMessage(int.Parse(acctId), action.Args[0]);
        }

        private void ForceTransport(int entityId, int roomId)
        {
            // LOOKUPS
            var entity = _entities[entityId];
            var oldRoom = GetRoomWithEntity(entityId);
            var newRoom = _rooms[roomId];
            var changeZone = oldRoom.Zone != newRoom.Zone;
            var oldZone = _zones[oldRoom.Zone];
            var newZone = _zones[newRoom.Zone];

            //PHYSICAL MOVEMENT
            if (changeZone)
            {
                //move entity between zones -- not used yet
            }

            oldRoom.Entities.Remove(entityId);
            newRoom.Entities.Add(entityId);
            entity.Traits.Set("room", newRoom.Id.ToString());

            //EVENT NOTIFICATIONS
            if (changeZone)
            {
                var leaveZone = new MudAction("leavezone", entityId, oldZone.Id);
                oldZone.DoAction(leaveZone);
                entity.DoAction(leaveZone);

                var enterZone = new MudAction("enterzone", entityId, newZone.Id);
                newZone.DoAction(enterZone);
                entity.DoAction(enterZone);
            }

            var leaveRoom = new MudAction("leaveroom", entityId, 0);
            oldRoom.DoAction(leaveRoom);
            entity.DoAction(leaveRoom);

            var enterRoom = new MudAction("enterroom", entityId, 0);
            newRoom.DoAction(enterRoom);
            ActionRoomMobs(enterRoom, newRoom.Id);
            ActionRoomItems(enterRoom, newRoom.Id);
        }

        private void Transfer(MudAction action)
        {
            /* entity to entity
               entity to room
               room to entity
            */

            //types of entities involved
            var e2e = _entities.ContainsKey(action.SenderId) && _entities.ContainsKey(action.ReceiverId);
            var e2r = _entities.ContainsKey(action.SenderId) && _rooms.ContainsKey(action.ReceiverId);
            var r2e = _rooms.ContainsKey(action.SenderId) && _entities.ContainsKey(action.ReceiverId);

            var requestor = r2e ? _rooms[action.SenderId] : _entities[action.SenderId];
            var receiver = e2r ? _rooms[action.ReceiverId] : _entities[action.ReceiverId];
            var subject = _entities[action.OtherEntity1];
            var zone = _zones[GetRoomWithEntity(subject.Id).Zone];

            var isItem = subject.Components.Has("item");
            var requestedQty = 1;
            int.TryParse(action.Args[0], out requestedQty);

            if (isItem && requestedQty > 1 && (e2e || e2r))
            {
                if (requestedQty > int.Parse(subject.Traits.Get("quantity").Value))
                {
                    DoAction(new MudAction("infotoplayer", 0, requestor.Id, "There aren't that many to pick up."));
                    return;
                }
            }

            //permission
            var canTake = new MudAction("cantake", requestor.Id, receiver.Id, subject.Id, action.Args);
            if (!subject.DoAction(canTake) || !receiver.DoAction(canTake) || !zone.DoAction(canTake) || !requestor.DoAction(canTake))
                return;

            //physical movement
            var newEntityId = 0;
            MudEntity newEntity;

            if (isItem && requestedQty > 1 && requestedQty != int.Parse(subject.Traits.Get("quantity").Value))
            {
                newEntity = _entityFactory.CreateEntity(subject.Name, (subject.Traits.GetAll().ToDictionary(x => x.Name, x => x.Value)));
                _entities.Add(newEntity.Id, newEntity);
                newEntityId = newEntity.Id;
                newEntity.Traits.Set("quantity", requestedQty.ToString());

                var comps = subject.Components.GetAll();
                foreach (var c in comps)
                    ComponentManager.Instance.AssignComponent(newEntity, c.Name);
            }
            else
            {
                if (r2e)
                    _rooms[requestor.Id].Entities.Remove(subject.Id);
                else if (e2r)
                {
                    var inventory = GetEntitiesFromTrait(requestor, "items").ToList();
                    inventory.Remove(subject);
                    requestor.Traits.Set("items", string.Join(",", inventory.Select(x => x.Id.ToString())));
                    _rooms[receiver.Id].Entities.Add(subject.Id);
                }
                
                newEntityId = subject.Id;
                newEntity = subject;
            }

            if (r2e)
            {
                //is subject an item?
                if (newEntity.Components.Has("item"))
                {
                    if (receiver.Traits.Has("items"))
                    {
                        var itemsString = receiver.Traits.Get("items").Value;

                        var itemIds = itemsString.Split(',').Select(x => int.Parse(x)).ToList();
                        itemIds.Add(subject.Id);
                        receiver.Traits.Set("items", string.Join(",", itemIds.Select(x => x.ToString())));
                    }
                    else
                    {
                        receiver.Traits.Add("items", newEntityId.ToString());
                    }
                }
            }
            else if (e2r)
            {
            }

            //notifications
            var transType = "";
            if (e2e)
                transType = "e2e";
            else if (e2r)
                transType = "e2r";
            else
                transType = "r2e";

            var receivedEntity = new MudAction("receivedentity", requestor.Id, receiver.Id, newEntity.Id, transType,
                newEntity.Traits.Has("quantity") ? newEntity.Traits.Get("quantity").Value : null);

            var room = r2e ? _rooms[requestor.Id] : GetRoomWithEntity(requestor.Id);
            ActionRoomMobs(receivedEntity, room.Id);
            ActionRoomItems(receivedEntity, room.Id);

            //TODO: CLEANUP ITEMS
            if (!isItem || !newEntity.Traits.Has("isStackable"))
                return;

            var entityItems = GetEntitiesFromTrait(receiver, "items").ToList();
            var existingItems = entityItems.Where(x => x.Name == newEntity.Name).ToList();

            if (existingItems.Count <= 1)
                return;

            var totalQty = existingItems.Sum(x => int.Parse(x.Traits.Get("quantity").Value));
            newEntity.Traits.Set("quantity", totalQty.ToString());

            var oldItem = existingItems.Single(x => x.Id != newEntity.Id);
            entityItems.Remove(oldItem);
            _entities.Remove(oldItem.Id);

            receiver.Traits.Set("items", string.Join(",", entityItems.Select(x => x.Id.ToString())));
        }

        public IEnumerable<MudEntity> GetEntitiesFromTrait(MudEntity entity, string trait)
        {
            var retVal = new List<MudEntity>();
            if (!entity.Traits.Has(trait))
                return retVal;

            var entitiesString = entity.Traits.Get(trait).Value;
            var ids = entitiesString.Split(',').Select(x => int.Parse(x)).ToList();

            retVal.AddRange(ids.Select(x => GetEntityById(x)));
            return retVal;
        }

        private void RouteActionToEntity(int entityId, MudAction action)
        {
            var mob = _entities[entityId];
            mob.DoAction(action);
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
            foreach (var player in players)
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