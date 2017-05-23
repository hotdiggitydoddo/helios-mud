using Helios.Domain.Models;
using Helios.Engine.Scripting;
using Helios.Engine.UI;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Text;
using Helios.Engine.Factories;
using Helios.Engine.Objects;

namespace Helios.Engine.Connections
{
    public class NewCharacterHandler : ConnectionHandler
    {
        private enum NewCharState 
        {
            EnteringName = 1,
            ChoosingRace,
            EnteringDescription
        }

        private NewCharState _state;
        private MudEntity _newChar;

        public NewCharacterHandler(Connection c, Account a) : base(c, a)
        {
            try
            {
                _script = new Script();
                _script.DoString(ScriptManager.Instance.GetScript(ScriptType.Game, "new-char"));
            }
            catch (KeyNotFoundException ex)
            {
                //Add logging
                throw ex;
            }
        }

        public override void Enter()
        {
            _state = NewCharState.EnteringName;
            _newChar = new MudEntity(-1);
            Game.Instance.SendMessage(_account.Id, _script.Call(_script.Globals["enter"]).String);
        }

        public override void Handle(string command)
        {
            if (command == "q")
            {
                 _connection.RemoveHandler();
                _connection.AddHandler<MainMenuHandler>();
                _connection.Handler.Enter();
                return;
            }

            switch (_state)
            {
                case NewCharState.EnteringName:
                    var name = char.ToUpper(command[0]) + command.Substring(1);
                    _newChar.Name = name;
                    
                    //test persistence
                    ComponentManager.Instance.AssignComponent(_newChar, "health", 
                        new MudTrait("maxHP", "50"), 
                        new MudTrait("currHP", "45"));
                    ComponentManager.Instance.AssignComponent(_newChar, "physical", 
                        new MudTrait("description", $"This is {_newChar.Name}'s description."), 
                        new MudTrait("weight", "175"));
                   
                    Game.Instance.SendMessage(_account.Id, $"Ah, welcome, {name}... 'tis a fine name, indeed.\n");
                    Game.Instance.SendMessage(_account.Id, _script.Call(_script.Globals["printRaces"]).String);

                    _newChar.Traits.Add("room", "3");

                    _newChar = Game.Instance.CreatePlayerCharacter(_account.Id, _newChar);

                    _state = NewCharState.ChoosingRace;
                break;

                case NewCharState.ChoosingRace:
                    Game.Instance.SendMessage(_account.Id, "Choosing races is hard!");
                break;

                case NewCharState.EnteringDescription:
                break;
            }
            
            if (command == "help" || command == "?")
                Game.Instance.SendMessage(_account.Id, "You asked for <#orange>main help<#>.");
            else if (command == "money")
                Game.Instance.SendMessage(_account.Id, "You asked for <#lightgreen>main $MONEY<#>.");
        }
        public override void Leave()
        {
            throw new NotImplementedException();
        }
    }
}
