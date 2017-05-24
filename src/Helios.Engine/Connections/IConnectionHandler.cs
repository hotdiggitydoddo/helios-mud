using System;
using System.Collections.Generic;
using System.Text;

namespace Helios.Engine.Connections
{
    public interface IConnectionHandler
    {
        void Enter(params object[] args);
        void Leave();
        void Handle(string command);
    }
}
