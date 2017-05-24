using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Helios.Engine.Actions;
using Helios.Engine.Connections;
using Helios.Engine.Containers;
using Helios.Engine.Locations;
using Helios.Engine.Objects;
using MoonSharp.Interpreter;

namespace Helios.Engine.Scripting
{
    public enum ScriptType
    {
        Game = 1,
        MudComponent,
        ActionRunner,
        MudCommand
    }

    public class ScriptManager
    {
        private static readonly ScriptManager _instance = new ScriptManager();
        public static ScriptManager Instance => _instance;

        private readonly Dictionary<string, string> _gameScripts;
        private readonly Dictionary<string, string> _componentScripts;
        private readonly Dictionary<string, string> _actionRunnerScripts;
        private readonly Dictionary<string, string> _commandScripts;

        private ScriptManager()
        {
            _componentScripts = new Dictionary<string, string>();
            _gameScripts = new Dictionary<string, string>();
            _actionRunnerScripts = new Dictionary<string, string>();
            _commandScripts = new Dictionary<string, string>();

            UserData.RegisterType<Script>();
            UserData.RegisterType<IMessageHandler>();
            UserData.RegisterType<Task>();
            UserData.RegisterType<MudCommand>();
            UserData.RegisterType<ComponentSet>();
            UserData.RegisterType<MudComponent>();
            UserData.RegisterType<TraitSet>();
            UserData.RegisterType<MudTrait>();
            UserData.RegisterType<Table>();
            UserData.RegisterType<Game>();
            //UserData.RegisterType<List<MudEntity>>();

            UserData.RegisterType<MudEntity>();
            UserData.RegisterType<List<MudEntity>>();
            UserData.RegisterType<MudAccount>();
            UserData.RegisterType<MudRoom>();
            //UserData.RegisterType<MudPortal>();
            UserData.RegisterType<MudZone>();
            UserData.RegisterType<List<MudEntity>>();
            UserData.RegisterType<List<MudRoom>>();
            //UserData.RegisterType<List<MudPortal>>();
            UserData.RegisterType<List<MudZone>>();
            UserData.RegisterType<MudWorld>();
            UserData.RegisterType<MudAction>();
        }

        // public bool AddScript(string name, string lua)
        // {
        //     if (_scripts.ContainsKey(name)) return false;
        //     _scripts.Add(name, lua);
        //     return true;
        // }

        // public bool RemoveScript(string name)
        // {
        //     if (!_scripts.ContainsKey(name)) return false;
        //     _scripts.Remove(name);
        //     return true;
        // }

        public string GetScript(ScriptType type, string name)
        {
            try
            {   switch(type)
                {
                    case ScriptType.ActionRunner:
                        return _actionRunnerScripts[name];
                    case ScriptType.Game:
                        return _gameScripts[name];
                    case ScriptType.MudComponent:
                        return _componentScripts[name];
                    case ScriptType.MudCommand:
                        return _commandScripts[name];
                    default:
                        return null;
                }
            }
            catch (KeyNotFoundException ex)
            {
                //TODO: log exception
                throw new KeyNotFoundException($"The script \"{name}\" was not found.");
            }
        }

        public Dictionary<string, string> GetCommandScripts()
        {
            return new Dictionary<string, string>(_commandScripts);
        }

        public Dictionary<string, string> GetComponentScripts()
        {
            return new Dictionary<string, string>(_componentScripts);
        }

        public void RefreshScripts(ScriptType type)
        {
            string[] files = new string[] {};

            switch(type)
            {
                case ScriptType.ActionRunner:
                    files = Directory.GetFiles(Path.Combine("GameScripts", "Actions"), "*.lua", SearchOption.AllDirectories);
                    _actionRunnerScripts.Clear();
                break;
                case ScriptType.Game:
                    files = Directory.GetFiles(Path.Combine("GameScripts", "Game"), "*.lua", SearchOption.AllDirectories);
                    _gameScripts.Clear();
                break;
                case ScriptType.MudComponent:
                    files = Directory.GetFiles(Path.Combine("GameScripts", "Components"), "*.lua", SearchOption.AllDirectories);
                    _componentScripts.Clear();
                break;
                 case ScriptType.MudCommand:
                    files = Directory.GetFiles(Path.Combine("GameScripts", "Commands"), "*.lua", SearchOption.AllDirectories);
                    _commandScripts.Clear();
                break;
            }

            foreach (var file in files)
            {
                var name = @file.Substring(@file.LastIndexOf(Path.DirectorySeparatorChar) + 1,
                    @file.LastIndexOf(".") - @file.LastIndexOf(Path.DirectorySeparatorChar) - 1);
                var script = System.IO.File.ReadAllText(file);

                if (type == ScriptType.Game)
                    _gameScripts.Add(name, script);
                else if (type == ScriptType.MudComponent)
                    _componentScripts.Add(name, script);
                else if (type == ScriptType.MudCommand)
                    _commandScripts.Add(name, script);
                else
                    _actionRunnerScripts.Add(name, script);
            }
        }

        // public void ReloadScript(string name)
        // {
        //     var files = Directory.GetFiles("GameScripts", $"{name}.lua", SearchOption.AllDirectories);
        //     if (files.Length == 0)
        //         return;

        //     var script = System.IO.File.ReadAllText(files[0]);
        //     _scripts[name] = script;

        // }
    }
}
