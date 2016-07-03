using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient.DotnetCore
{
    public class ResponseEventArgs : System.EventArgs
    {
        public byte[] ResponseMsg { get; set; }

        public int ResponseLength { get; set; }

        public ResponseEventArgs(byte[] responseMsg, int responseLength)
        {
            ResponseMsg = responseMsg;
            ResponseLength = responseLength;
        }
    }
}