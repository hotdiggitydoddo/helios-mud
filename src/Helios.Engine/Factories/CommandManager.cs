using System;
using System.Collections.Generic;
using System.Linq;
using Helios.Engine.Actions;
using Helios.Engine.Containers;
using Helios.Engine.Objects;
using Helios.Engine.Scripting;
using MoonSharp.Interpreter;

namespace Helios.Engine.Factories
{
    public class CommandManager
    {
        private Dictionary<int, List<string>> _entityCommands;
        private Dictionary<string, Script> _commandSet;
         
        public CommandManager()
        {
            _commandSet = new Dictionary<string, Script>();
            _entityCommands = new Dictionary<int, List<string>>();
        }

        // public void AddNewCommand(string name, string pathToScript)
        // {
        //     var script = new Script();
        //     //script.Globals["Command"] = typeof(MudCommand);
        //     script.Globals["Game"] = typeof(Game);
        //     script.Globals["MudAction"] = typeof(MudAction);
        //     script.DoFile(pathToScript);

        //     if (!_commandSet.ContainsKey(name))
        //         _commandSet.Add(name, script);
        // }

        public void LoadAllCommands()
        {
            _commandSet.Clear();
            var allScripts = ScriptManager.Instance.GetCommandScripts();
            
            foreach(var kvp in allScripts)
            {
                 var script = new Script();
                 //script.Globals["Command"] = typeof(MudCommand);
                 script.Globals["Game"] = typeof(Game);
                
                    script.Globals["MudAction"] = typeof(MudAction);

              
                try
                {
                 script.DoString(kvp.Value);

                }
                  catch(Exception ex)
                {
                    
                }
                _commandSet.Add(kvp.Key, script);
            }
        }

        public void AssignCommand(int entityId, string cmdName)
        {
            if (!_entityCommands.ContainsKey(entityId))
                _entityCommands.Add(entityId, new List<string>{ cmdName });
            else if (!_entityCommands[entityId].Contains(cmdName))
                _entityCommands[entityId].Add(cmdName);
        }

        public List<string> GetCommands(int entityId)
        {
            if (!_entityCommands.ContainsKey(entityId))
                return new List<string>();
            return new List<string>(_entityCommands[entityId]);
        }

        public void Process(int entityId, string input)
        {
            if (!Parse(entityId, ref input))
                return;
            
            var parts = input.Trim().Split(' ').ToList();
            var verb = parts[0].ToLower();
            parts.RemoveAt(0);

            Execute(entityId, verb, parts.ToArray());
        }

        private bool Parse(int entityId, ref string input)
        {
            //this method should really return a custom object that has the command name and
            //typed and named arguments

            if (!_entityCommands.ContainsKey(entityId))
                return false;

            var parts = input.Trim().Split(' ');
            var verb = parts[0].ToLower();

            if (verb == "commands")
                //need to communicate available commands to player.
                return false;

            if (!_commandSet.ContainsKey(verb))
                //Send error to client -- command doesn't exist
                return false;
            if (!_entityCommands[entityId].Contains(verb))
                //entity doesn't know this command
                return false;
            return true;
        }

        private void Execute(int entityId, string cmdName, params string[] args)
        {
            var script = _commandSet[cmdName];
            try
            {
                script.Call(script.Globals["execute"], entityId, args);
            }
            catch (ScriptRuntimeException ex)
            {
                
            }
        }
    }
}
