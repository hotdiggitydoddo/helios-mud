using Helios.Engine.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using Helios.Engine.Locations;
using MoonSharp.Interpreter;
using System.Linq;

namespace Helios.Engine.Objects
{
    public class ReporterComponent : MudComponent
    {
        private int _acctId;
        public ReporterComponent(MudEntity owner, string name, Script script) : base(owner, name, script)
        {
            _acctId = int.Parse(owner.Traits.Get("accountId")?.Value);
            IsActive = true;
        }

        public override bool DoAction(MudAction action)
        {
            if (action.Type == "enterrealm")
            {
                var mob = Game.Instance.GetEntityById(action.SenderId);
                Game.Instance.SendMessage(_acctId, $"{mob.Name} has entered the realm.");
            }
            else if (action.Type == "leaverealm")
            {
                var mob = Game.Instance.GetEntityById(action.SenderId);
                Game.Instance.SendMessage(_acctId, $"{mob.Name} has left the realm.");
            }
            else if (action.Type == "enterroom")
                EnterRoom(action);
            else if (action.Type == "look")
                EnterRoom(action);
            else if (action.Type == "say")
                Game.Instance.SendMessage(_acctId, $"{action.Args[1]} says, \"{action.Args[0]}\"");
            else if (action.Type == "listinventory")
                SeeInventory(action.SenderId);
            else if (action.Type == "receivedentity")
                ReceivedEntity(action);
            return true;
        }

        private void ReceivedEntity(MudAction action)
        {
            var transType = action.Args[0];
            var subject = Game.Instance.GetEntityById(action.OtherEntity1);

            if (transType == "r2e")
            {
                var receiver = Game.Instance.GetEntityById(action.ReceiverId);
                if (receiver.Id == Owner.Id)
                    Game.Instance.SendMessage(_acctId, $"You pick up {subject.Name}.");
                else
                    Game.Instance.SendMessage(_acctId, $"{receiver.Name} picks up {subject.Name}.");
            }
        }

        private void EnterRoom(MudAction action)
        {
            var mobId = action.SenderId;
            if (mobId == Owner.Id)
            {
                SeeRoom(int.Parse(Owner.Traits.Get("room").Value), mobId);
                return;
            }

            var mob = Game.Instance.GetEntityById(mobId);
            var portalId = action.ReceiverId;

            if (portalId == 0)
            {
                Game.Instance.SendMessage(_acctId, $"{mob.Name} has entered.");
                return;
            }

            var portal = Game.Instance.GetPortalById(portalId);
            Game.Instance.SendMessage(_acctId, $"{mob.Name} enters from the {portal.Name}.");
        }

        private void SeeRoom(int roomId, int mobId)
        {
            var room = Game.Instance.GetRoomById(roomId);
            var mob = Game.Instance.GetEntityById(mobId);

            SeeRoomName(room);
            SeeRoomDesc(room);
            SeeRoomExits(room);
            SeeRoomMobs(room);
            SeeRoomItems(room);
        }

        private void SeeInventory(int mobId)
        {
            var mob = Game.Instance.GetEntityById(mobId);
            var itemsString = mob.Traits.Get("items")?.Value;

            if (string.IsNullOrWhiteSpace(itemsString))
                {
                    Game.Instance.SendMessage(_acctId, "You are carrying nothing.");
                    return;
                }
            var itemIds = itemsString.Split(',').Select(x => int.Parse(x)).ToList();
            var items = new List<MudEntity>();

            foreach(var id in itemIds)
            {
               items.Add(Game.Instance.GetEntityById(id));
            }
            var msg = ConcatEntities(items);
            Game.Instance.SendMessage(_acctId, $"You are carrying: {msg}.");
        }

        private void SeeRoomItems(MudRoom room)
        {
            var items = Game.Instance.GetEntitiesInRoom(room.Id, x => !x.Traits.Has("race") && x.Traits.Has("item"));
            if (!items.Any())
            {
                Game.Instance.SendMessage(_acctId, "There is nothing of note here.");
                return;
            }

            var sb = new StringBuilder();
            Game.Instance.SendMessage(_acctId, $"You see {ConcatEntities(items)} on the ground.");
        }

        private void SeeRoomMobs(MudRoom room)
        {
            var mobs = Game.Instance.GetEntitiesInRoom(room.Id, x => x.Traits.Has("race") && !x.Traits.Has("item") && x.Id != Owner.Id);
            if (!mobs.Any())
            {
                Game.Instance.SendMessage(_acctId, "There are no signs of life here.");
                return;
            }
            var sb = new StringBuilder();
            var verb = mobs.Count > 2 ? "are" : "is";
            Game.Instance.SendMessage(_acctId, $"{ConcatEntities(mobs)} {verb} here with you.");
        }

        private void SeeRoomExits(MudRoom room)
        {
            //throw new NotImplementedException();
        }

        private void SeeRoomName(MudRoom room)
        {
            Game.Instance.SendMessage(_acctId, $"{room.Name}");
        }

        private void SeeRoomDesc(MudRoom room)
        {
            Game.Instance.SendMessage(_acctId, $"{room.Traits.Get("description").Value}");
        }

        private string ConcatEntities(List<MudEntity> entities)
        {
            if (entities.Count == 1)
                return entities[0].Name;
            //0, 1, 2, 3
            var sb = new StringBuilder();

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i].Id == Owner.Id)
                    continue;
                if (i == entities.Count - 1 && i != 0)
                    sb.Append("and ").Append(entities[i].Name);
                else
                    sb.Append(entities[i].Name).Append(", ");
            }
            return sb.ToString();
        }
    }
}