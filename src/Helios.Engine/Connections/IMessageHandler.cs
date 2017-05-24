using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Engine.Connections
{
    public interface IMessageHandler
    {
        Task SendToAll(string message, params string[] args);
        Task SendToAccount(int accountId, string message, params string[] args);
        Task Logout(int accountId);
    }
}
