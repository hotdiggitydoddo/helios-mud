using System;
using Helios.Domain.Models;

namespace Helios.Engine.Connections
{
    public class Connection
    {
        public string Id { get; }
        public Account Account {get;}
        public IConnectionHandler Handler { get; private set; }
        public ConnectionState State {get; private set;}

        public Connection(string id, Account account)
        {
            Id = id;
            Account = account;
            State = ConnectionState.Login;
        }

        public void AddHandler<T>(params object[] args) where T : ConnectionHandler
        {
            if (Handler != null)
                throw new System.Exception("Can't add a handler before removing the existing one.");
            Handler = (T)Activator.CreateInstance(typeof(T), this, Account, args);
        }

        public void RemoveHandler()
        {
            Handler = null;
        }

    }
}