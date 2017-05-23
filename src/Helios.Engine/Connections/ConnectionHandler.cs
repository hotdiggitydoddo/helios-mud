using Helios.Domain.Models;
using MoonSharp.Interpreter;

namespace Helios.Engine.Connections
{
    public abstract class ConnectionHandler : IConnectionHandler
    {
        protected Account _account;
        protected Script _script;
        protected Connection _connection;

        public ConnectionHandler(Connection c, Account a, params object[] args)
        {
            _account = a;
            _connection = c;
        }

        public abstract void Enter();
        public abstract void Handle(string command);
        public abstract void Leave();
    }
}
