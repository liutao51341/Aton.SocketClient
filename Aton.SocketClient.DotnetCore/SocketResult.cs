using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient.DotnetCore
{
    public class SocketResult
    {
        public SocketResult() { Result = false; Desc = string.Empty; }
        public bool Result { get; set; }

        public string Desc { get; set; }
    }
}
