using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient.DotnetCore
{
    public class SocketClientFactory
    {
        public static ISocketClient CreateSocketServer(ServerMode serverMode)
        {
            if (serverMode == ServerMode.TCP)
                return new TcpSocketClient();
            else if (serverMode == ServerMode.UDP)
                return new UdpSocketClient();
            else if (serverMode == ServerMode.AtonUDP)
                return new UdpAtonSocketClient();
            else
                return null;
        }
    }
}
