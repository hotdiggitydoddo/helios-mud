using Helios.Engine.Actions;
using MoonSharp.Interpreter;

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
            return true;
        }

    }
}