using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace Aton.SocketClient
{
    public abstract class SocketClientBase : ISocketClient
    {
        public string ServerIpAddress
        {
            get; set;
        }

        public int ServerPort
        {
            get; set;
        }

        public SocketClientBase(string ipAddress, int port)
        {
            ServerIpAddress = ipAddress;
            ServerPort = port;
        }

        public SocketClientBase()
        {
        }

        public abstract event EventHandler<ResponseEventArgs> OnResponseRecv;

        public abstract event EventHandler<ConnectedEventArgs> OnConnected;

        public abstract SocketResult Connect(byte[] data = null);

        public abstract SocketResult DisConnect();

        public abstract SocketResult RequestAsync(byte[] requestMsg);

        public abstract SocketResult RequestSync(byte[] requestMsg, ref byte[] responseMsg);


    }
}
