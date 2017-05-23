using System;
using System.Collections.Generic;
using Helios.Domain.Models;
using Helios.Engine.Connections;

namespace Helios.ConsoleApp
{
    public class HeliosConsole
    {
        private ConsoleMessageHandler _handler;        
        private Account _account;
        private string _connectionId;
        private bool _quit;

        public void Init()
        {
            _handler = new ConsoleMessageHandler();
            _account = new Account
            {
                Id = 1,
                UserId = 1,
                Characters = new List<Account_Entity>(),
                IsBanned = false
            };
            _connectionId = Guid.NewGuid().ToString();
        }

        public void Login()
        {
            _handler.Login(_connectionId, _account);

            while(!_quit)
            {
                Console.Write("Your command?: ");
                var input = Console.ReadLine();
                if (input == "Q") 
                    return;
                _handler.ReceiveMessage(_connectionId, input);
            }
        }
    }
}