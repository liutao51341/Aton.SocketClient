

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient
{
    public interface ISocketClient
    {
        event EventHandler<ResponseEventArgs> OnResponseRecv;

        event EventHandler<ConnectedEventArgs> OnConnected;
        string ServerIpAddress { get; set; }

        int ServerPort { get; set; }
        SocketResult Connect(byte[] data = null);

        SocketResult DisConnect();

        SocketResult RequestSync(byte[] requestMsg, ref byte[] responseMsg);

        SocketResult RequestAsync(byte[] requestMsg);
    }
}
