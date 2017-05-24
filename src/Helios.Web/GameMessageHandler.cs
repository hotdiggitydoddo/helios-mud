using Helios.Domain.Models;
using Helios.Engine;
using Helios.Engine.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;
using Helios.Engine.UI;
using System.Threading;
//using Helios.Engine.World;

namespace Helios.Web
{
    public class GameMessageHandler : WebSocketHandler, IMessageHandler
    {
        private List<Connection> _connections;
        private Thread _gameThread;
        public GameMessageHandler(WebSocketConnectionManager webSocketConnectionManager)
            : base(webSocketConnectionManager)
        {
            Game.Instance.Init(this);
            _gameThread = new Thread(Game.Instance.Start);
            _gameThread.Start();
            _connections = new List<Connection>();
        }

        public override async Task OnConnectedAsync(WebSocket socket)
        {
            await base.OnConnectedAsync(socket);
            var connectionId = WebSocketConnectionManager.GetId(socket);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            // var connectionId = WebSocketConnectionManager.GetId(socket);

            // var conn = _connections.Single(x => x.Id == connectionId);
            // if (conn.Handler != null)
            // {
            //     conn.Handler.Leave();
            // }

            

            // _connections.RemoveAll(x => x.Id == connectionId);

            await base.OnDisconnected(socket);
        }

        public async Task SendToAll(string message, params string[] args)
        {
            await InvokeClientMethodToAllAsync("receiveMessage", message, args);
        }

        public async Task SendToAccount(int accountId, string message, params string[] args)
        {
            var conn = _connections.Single(x => x.Account.Id == accountId);
            await InvokeClientMethodAsync(conn.Id, "receiveMessage", new[] { message });
        }

        public async Task ReceiveMessage(string connectionId, string message)
        {
            var conn = _connections.Single(x => x.Id == connectionId);
            if (conn.Handler != null)
            {
                // if (message == "create")
                // {
                //     var e = _entityService.Create();
                //     var t = new Dictionary<string, string>();
                //     t.Add("Health", "155");
                //     var e1 = _entityService.Create(t);
                //     var e2 = 
                // }
                //get a IConnectionHandler result or null - if the result
                if (message == "#")
                {
                    Engine.Scripting.ScriptManager.Instance.RefreshScripts(Engine.Scripting.ScriptType.Game);
                    await InvokeClientMethodAsync(conn.Id, "receiveMessage", new[] { "DEBUG: refreshed game scripts." });
                }
                else
                    conn.Handler.Handle(message.ToLower());
            }
            else
            {
                //pass to game
            }
        }

        public async void Login(string connectionId, Account account)
        {
            if (_connections.Any(x => x.Account.Id == account.Id))
            {
               await InvokeClientMethodAsync(connectionId, "receiveMessage", new[] { "<span style=\"color:red;\">You are already connected to the game in another browser window.  Please close this window and return to the original one.</span>"});
               return;
            }

            var connection = new Connection(connectionId, account);
            _connections.Add(connection);
            connection.AddHandler<LoginHandler>();
            connection.Handler.Enter();
        }

        public void Logout(string connectionId)
        {
            var conn = _connections.Single(x => x.Id == connectionId);
            if (conn.Handler != null)
            {
                conn.Handler.Leave();
            }

            _connections.RemoveAll(x => x.Id == connectionId);
        }
    }
}
