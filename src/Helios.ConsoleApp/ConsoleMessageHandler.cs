using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helios.Domain.Models;
using Helios.Engine;
using Helios.Engine.Connections;

namespace Helios.ConsoleApp
{
     public class ConsoleMessageHandler : IMessageHandler
    {
        private List<Connection> _connections;
        private Thread _gameThread;
        public ConsoleMessageHandler()
        {
            Game.Instance.Init(this);
            _gameThread = new Thread(Game.Instance.Start);
            _gameThread.Start();
            _connections = new List<Connection>();
        }

        // public async Task OnConnectedAsync(WebSocket socket)
        // {
        //     await base.OnConnectedAsync(socket);
        //     var connectionId = WebSocketConnectionManager.GetId(socket);
        // }

        // public async Task OnDisconnected(WebSocket socket)
        // {
        //     var connectionId = WebSocketConnectionManager.GetId(socket);
        //     _connections.RemoveAll(x => x.Id == connectionId);

        //     await base.OnDisconnected(socket);
        // }

        public async Task SendToAll(string message, params string[] args)
        {
            //await InvokeClientMethodToAllAsync("receiveMessage", message, args);
            return;
        }

        public async Task SendToAccount(int accountId, string message, params string[] args)
        {
            var conn = _connections.Single(x => x.Account.Id == accountId);
            //await InvokeClientMethodAsync(conn.Id, "receiveMessage", new[] { message });
            System.Console.WriteLine(message);
        }

        public async Task ReceiveMessage(string connectionId, string message)
        {
            var conn = _connections.Single(x => x.Id == connectionId);
            if (conn.Handler != null)
            {
                if (message == "#")
                {
                    Engine.Scripting.ScriptManager.Instance.RefreshScripts(Engine.Scripting.ScriptType.Game);
                    Console.WriteLine("DEBUG: refreshed game scripts." );
                }
                else
                    //get a IConnectionHandler result or null - if the result
                    conn.Handler.Handle(message);
            }
            else
            {
                //pass to game
            }
        }

        public async Task Login(string connectionId, Account account)
        {
            if (_connections.Any(x => x.Account.Id == account.Id))
            {
               //await InvokeClientMethodAsync(connectionId, "receiveMessage", new[] { OutputHtml.Write("<#red>You are already connected to the game in another browser window.  Please close this window and return to the original one.")});
               //await OnDisconnected(WebSocketConnectionManager.GetSocketById(connectionId));
               return;
            }

            var connection = new Connection(connectionId, account);
            _connections.Add(connection);
            connection.AddHandler<LoginHandler>();
            connection.Handler.Enter();
        }

        public Task Logout(int accountId)
        {
            var conn = _connections.Single(x => x.Account.Id == accountId);
            if (conn.Handler == null) return Task.FromResult(0);

            conn.RemoveHandler();
            conn.AddHandler<MainMenuHandler>();
            conn.Handler.Enter();

            return Task.FromResult(1);
        }
    }
}