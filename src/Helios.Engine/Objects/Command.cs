using MoonSharp.Interpreter;

namespace Helios.Engine.Objects
{
    public class Command
    {
        private readonly Script _script;

        public int EntityId { get; }
        public string Name { get; }
        public string Usage { get; }
        public string Description { get; }


        public bool Execute(params object[] args)
        {
            return _script.Call(_script.Globals["execute"], args).Boolean;
        }

        public Command(int entityId, string name, string usage, string description, Script script)
        {
            EntityId = entityId;
            Name = name;
            Usage = usage;
            Description = description;
            _script = script;
        }
    }
}
