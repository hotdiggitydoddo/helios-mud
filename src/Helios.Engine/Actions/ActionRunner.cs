using Helios.Engine.Scripting;
using MoonSharp.Interpreter;

namespace Helios.Engine.Actions
{
    public class ActionRunner
    {
        private Script _script;
        public string ActionName {get;}

        public ActionRunner(string name)
        {
            ActionName = name;
        }

        public void Init()
        {
            var script = new Script();
            script.Globals["MudAction"] = typeof(MudAction);
            script.DoString(ScriptManager.Instance.GetScript(ScriptType.ActionRunner, ActionName));
            _script = script;
        }
        
        public void Run(MudAction action)
        {
            _script.Call(_script.Globals["run"], action);
        }

    }
}