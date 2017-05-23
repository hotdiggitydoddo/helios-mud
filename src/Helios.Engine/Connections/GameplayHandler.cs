using System;
using Helios.Domain.Models;
using Helios.Engine.Actions;
using Helios.Engine.Objects;

namespace Helios.Engine.Connections
{
    public class GameplayHandler : ConnectionHandler
    {
        private MudEntity _player;

        public GameplayHandler(Connection c, Account a, params object[] args) : base(c, a)
        {
            _player = (MudEntity)args[0];
        }

        public override void Enter()
        {
            Game.Instance.SendMessage(_account.Id, $"Welcome to the game, {_player.Name}.");
            Game.Instance.DoAction(new MudAction("enterworld", _player.Id));
        }

        public override void Handle(string command)
        {
            //throw new NotImplementedException();
        }

        public override void Leave()
        {
            //throw new NotImplementedException();
        }
    }
}