using Helios.Domain.Models;
using Helios.Engine.Scripting;
using Helios.Engine.UI;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helios.Engine.Connections
{
    enum LoginState
    {

    }
    public class LoginHandler : ConnectionHandler
    {
        public LoginHandler(Connection c, Account a, params object[] args) : base(c, a, args)
        {
            try
            {
                _script = new Script();
                _script.DoString(ScriptManager.Instance.GetScript(ScriptType.Game, "login"));
            }
            catch (KeyNotFoundException ex)
            {
                //Add logging
                throw ex;
            }
        }

        public override void Enter()
        {
            Game.Instance.SendMessage(_account.Id, _script.Call(_script.Globals["getTitle"]).String);
            Game.Instance.SendMessage(_account.Id, _script.Call(_script.Globals["motd"]).String);
            _connection.RemoveHandler();
            _connection.AddHandler<MainMenuHandler>();
            _connection.Handler.Enter();
        }

        public override void Handle(string command) { }

        public override void Leave() { }
    }
}
