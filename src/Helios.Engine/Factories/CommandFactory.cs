using System.Collections.Generic;
using Helios.Engine.Objects;
using MoonSharp.Interpreter;

namespace Helios.Engine.Factories
{
    public class CommandFactory
    {
        private readonly Dictionary<string, string> _commands;
         
        public CommandFactory()
        {
            _commands = new Dictionary<string, string>();
        }

        public void AddCommand(string name, string pathToScript)
        {
            if (!_commands.ContainsKey(name))
                _commands.Add(name, pathToScript);
        }

        public void AssignCommand(MudEntity entity, string cmdName)
        {
            var script = new Script();
            script.Globals["Command"] = typeof(Command);
            script.DoFile(_commands[cmdName]);

            var cmd = (Command)script.Call(script.Globals["init"], entity.Id, script).UserData.Object;
            //entity.Commands.Add(cmd);
        }
    }
}
