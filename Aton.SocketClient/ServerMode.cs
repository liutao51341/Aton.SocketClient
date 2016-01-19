using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient
{
    /// <summary>
    /// 网络模式
    /// </summary>
    public enum ServerMode : int
    {
        /// <summary>
        /// TCP模式
        /// </summary>
        TCP=1,
        /// <summary>
        /// UDP模式
        /// </summary>
        UDP=2,
        /// <summary>
        /// Aton双Socket模式
        /// </summary>
        AtonUDP=3

    }
}
