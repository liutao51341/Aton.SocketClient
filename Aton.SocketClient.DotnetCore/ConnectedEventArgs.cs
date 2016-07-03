using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Aton.SocketClient.DotnetCore
{
    public class ConnectedEventArgs : System.EventArgs
    {
        public EndPoint RemoteEndPoint { get; set; }

        public byte[] ConnectRspData { get; set; }
 
        public ConnectedEventArgs(EndPoint remoteEndPoint, byte[] connectRspData)
        {
            RemoteEndPoint = remoteEndPoint;
            ConnectRspData = connectRspData;
        }
    }
}