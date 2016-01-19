using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aton.SocketClient
{
    /// <summary>
    /// server socket mode
    /// </summary>
    public enum ServerMode : int
    {
        /// <summary>
        /// TCP mode
        /// </summary>
        TCP=1,
        /// <summary>
        /// UDP mode
        /// </summary>
        UDP=2,
        /// <summary>
        /// Aton double Socket mode
        /// </summary>
        AtonUDP=3

    }
}
