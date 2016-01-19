using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Aton.SocketClient
{
    public class TcpSocketClient : SocketClientBase
    {
        Socket m_Socket;

        SocketAsyncEventArgs m_SocketAsyncEventArgs;

        byte[] m_SocketBuffer;

        public override event EventHandler<ResponseEventArgs> OnResponseRecv;

        public override event EventHandler<ConnectedEventArgs> OnConnected;

        public TcpSocketClient(string ipAddress, int port) : base(ipAddress, port)
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_SocketBuffer = new byte[1024];

            m_SocketAsyncEventArgs = new SocketAsyncEventArgs();
            m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, 0);
            m_SocketAsyncEventArgs.Completed += M_SocketAsyncEventArgs_Completed;
        }

        public TcpSocketClient()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_SocketBuffer = new byte[1024];

            m_SocketAsyncEventArgs = new SocketAsyncEventArgs();
            m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, 0);
            m_SocketAsyncEventArgs.Completed += M_SocketAsyncEventArgs_Completed;
        }

        private void M_SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive://recv finish
                    ProcessRecv(e);
                    break;
                case SocketAsyncOperation.Send://sent finish
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
                if (!m_Socket.ReceiveAsync(m_SocketAsyncEventArgs))
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

        public override SocketResult Connect(byte[] data = null)
        {
            SocketResult sr = new SocketResult();
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ServerIpAddress), ServerPort);
            try
            {
                m_Socket.Connect(remoteEndPoint);
                sr.Result = true;
                if (OnConnected != null)
                {
                    OnConnected(this, new ConnectedEventArgs(remoteEndPoint, m_SocketBuffer));
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
                m_Socket.Disconnect(true);
                sr.Result = true;
            }
            catch (Exception ex)
            {
                sr.Desc = ex.Message;
            }
            return sr;
        }

        public override SocketResult RequestAsync(byte[] requestMsg)
        {
            SocketResult sr = new SocketResult();
            if (string.IsNullOrEmpty(ServerIpAddress) || ServerPort == 0)
            {
                sr.Desc = "IP or Port not Set";
            }
            else if (!m_Socket.Connected)
            {
                sr.Desc = "Server not connected";
            }
            else if (OnResponseRecv == null)
            {
                sr.Desc = "not bind event on Async call";
            }
            else
            {
                m_SocketAsyncEventArgs.SetBuffer(m_SocketBuffer, 0, requestMsg.Length);
                Buffer.BlockCopy(requestMsg, 0, m_SocketBuffer, 0, requestMsg.Length);
           
                if (!m_Socket.SendAsync(m_SocketAsyncEventArgs))
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
            if (string.IsNullOrEmpty(ServerIpAddress) || ServerPort == 0)
            {
                sr.Desc = "IP or Port not Set";
            }
            else if (!m_Socket.Connected)
            {
                sr.Desc = "Server not connected";
            }
            else if (responseMsg == null)
            {
                sr.Desc = "Recv response buffer is null";
            }
            else
            {
                Array.Clear(responseMsg, 0, responseMsg.Length);
                if (m_Socket.Send(requestMsg) == requestMsg.Length)
                {
                    sr.Result = m_Socket.Receive(responseMsg) > 0;
                }
            }
            return sr;
        }

    }
}
