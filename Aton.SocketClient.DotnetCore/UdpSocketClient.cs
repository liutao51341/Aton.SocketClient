using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aton.SocketClient.DotnetCore
{
    public class UdpSocketClient : SocketClientBase
    {
        Socket m_Socket;

        SocketAsyncEventArgs m_SocketAsyncEventArgs;

        byte[] m_SocketBuffer;

        int DataServerPort;

        public override event EventHandler<ResponseEventArgs> OnResponseRecv;

        public override event EventHandler<ConnectedEventArgs> OnConnected;

        public UdpSocketClient(string ipAddress, int port) : base(ipAddress, port)
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_SocketBuffer = new byte[1024];

            m_SocketAsyncEventArgs = new SocketAsyncEventArgs();
            m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, 0);
            m_SocketAsyncEventArgs.Completed += M_SocketAsyncEventArgs_Completed;
        }

        public UdpSocketClient()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_SocketBuffer = new byte[1024];

            m_SocketAsyncEventArgs = new SocketAsyncEventArgs();
            m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, 0);
            m_SocketAsyncEventArgs.Completed += M_SocketAsyncEventArgs_Completed;
        }

        private void M_SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessRecv(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessRequest(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive");
            }
        }

        private void ProcessRequest(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                Array.Clear(m_SocketBuffer, 0, m_SocketBuffer.Length);
                if (!m_Socket.ReceiveFromAsync(m_SocketAsyncEventArgs))
                {
                    ProcessRecv(m_SocketAsyncEventArgs);
                }
            }
        }

        private void ProcessRecv(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                if (OnResponseRecv != null)
                {
                    OnResponseRecv(this, new ResponseEventArgs(e.Buffer, e.BytesTransferred));
                }
            }
        }

        public override SocketResult Connect(byte[] data)
        {
            SocketResult sr = new SocketResult();
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPort);
            try
            {
                if (m_Socket.SendTo(data, remoteEndPoint) == data.Length)
                {
                    int recvNum = m_Socket.ReceiveFrom(m_SocketBuffer, ref remoteEndPoint);
                    if (recvNum>0 && OnConnected != null)
                    {
                        OnConnected(this, new ConnectedEventArgs(remoteEndPoint, m_SocketBuffer));
                    }
                    Array.Clear(m_SocketBuffer, 0, m_SocketBuffer.Length);
                }
            }
            catch (Exception ex)
            {
                sr.Desc = ex.Message;
            }
            return sr;
        }

        public override SocketResult DisConnect()
        {
            SocketResult sr = new SocketResult();
            try
            {
                DataServerPort = 0;
                sr.Result = true;
            }
            catch (Exception ex)
            {
                sr.Desc = ex.Message;
            }
            return sr;
        }

        private void StartRecv()
        {

        }

        public override SocketResult RequestAsync(byte[] requestMsg)
        {
            SocketResult sr = new SocketResult();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), DataServerPort);
            if (string.IsNullOrEmpty(ServerIpAddress) || ServerPort == 0)
            {
                sr.Desc = "IP or Port not Set";
            }
            else if (OnResponseRecv == null)
            {
                sr.Desc = "not bind event on Async call";
            }
            else
            {
                m_SocketAsyncEventArgs.RemoteEndPoint = remoteEndPoint;
                m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, requestMsg.Length);
                Buffer.BlockCopy(requestMsg, 0, m_SocketBuffer, 0, requestMsg.Length);
                if (!m_Socket.SendToAsync(m_SocketAsyncEventArgs))
                {
                    ProcessRecv(m_SocketAsyncEventArgs);
                }
                sr.Result = true;
            }
            return sr;
        }


        public override SocketResult RequestSync(byte[] requestMsg, ref byte[] responseMsg)
        {
            SocketResult sr = new SocketResult();
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), DataServerPort);
            if (string.IsNullOrEmpty(ServerIpAddress) || DataServerPort == 0)
            {
                sr.Desc = "IP or Port not Set";
            }
            else if (responseMsg == null)
            {
                sr.Desc = "Recv response buffer is null";
            }
            else
            {
                Array.Clear(responseMsg, 0, responseMsg.Length);
                if (m_Socket.SendTo(requestMsg, remoteEndPoint) == requestMsg.Length)
                {
                    sr.Result = m_Socket.ReceiveFrom(responseMsg, ref remoteEndPoint) > 0;
                }
            }
            return sr;
        }
    }
}