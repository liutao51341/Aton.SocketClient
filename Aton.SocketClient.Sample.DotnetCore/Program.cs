using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aton.SocketClient.Sample.DotnetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Aton.SocketClient.DotnetCore.TcpSocketClient client = new SocketClient.DotnetCore.TcpSocketClient();
            client.ServerIpAddress = "192.168.10.103";
            client.ServerPort = 5000;
            var r=client.Connect(new byte[] { 88,77,88,106});
            Console.WriteLine(r.Desc);
            byte[] re = new byte[1024];
            var xclient=client.RequestSync(new byte[] { 88, 77, 88, 106 },ref re);
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(re));
            Console.Read();
        }
    }
}
