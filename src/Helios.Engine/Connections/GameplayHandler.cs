using System;
using Helios.Domain.Models;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Connections
{
    public class GameplayHandler : ConnectionHandler
    {
        private MudEntity _player;

        public GameplayHandler(Connection c, Account a) : base(c, a) { }

        public override void Enter(params object[] args)
        {
            var playerId = (int)args[0];

            Game.Instance.DoAction(new MudAction("enterworld", playerId));
            _player = Game.Instance.GetEntityById(playerId);
        }

        public override void Handle(string command)
        {   
            Game.Instance.Commands.Process(_player.Id, command);
        }

        public override void Leave()
        {
            Game.Instance.DoAction(new MudAction("leaveworld", _player.Id));
        }
    }
}