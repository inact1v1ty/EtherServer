using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EtherServer.Networking
{
    public class NetClient
    {
        TcpClient tcpClient;
        
        byte[] rbuffer = new byte[1024];

        public delegate void ReceivedCallback(byte[] buffer, int length);

        public event ReceivedCallback OnReliableReceived;
        public event ReceivedCallback OnUnReliableReceived;

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return tcpClient.Client.RemoteEndPoint as IPEndPoint;
            }
        }

        public NetClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            var port = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Port;
        }

        public void BeginTcpReceive()
        {
            try {
                tcpClient.Client.BeginReceive(rbuffer, 0, rbuffer.Length, SocketFlags.None, OnReceivedReliable, null);
            }
            catch (SocketException ex)
            {
                Close();
                NetServer.Instance.ClientDisconnected(this.RemoteEndPoint);
            }
        }

        private void OnReceivedReliable(IAsyncResult ar)
        {
            try
            {
                int read = tcpClient.Client.EndReceive(ar);

                if (read == 0)
                    return;
                
                OnReliableReceived?.Invoke(rbuffer, read);

                tcpClient.Client.BeginReceive(rbuffer, 0, rbuffer.Length, SocketFlags.None, OnReceivedReliable, null);
            }
            catch (SocketException ex)
            {
                Close();
                NetServer.Instance.ClientDisconnected(this.RemoteEndPoint);
            }
            catch (System.Exception)
            {
                Close();
            }
        }

        internal void ReceivedUdp(byte[] buffer)
        {
            OnUnReliableReceived?.Invoke(buffer, buffer.Length);
        }

        public void SendReliable(byte[] buffer)
        {
            try
            {
                tcpClient.Client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, OnSendReliable, null);
            }
            catch (SocketException ex)
            {
                Close();
                NetServer.Instance.ClientDisconnected(this.RemoteEndPoint);
            }
        }

        private void OnSendReliable(IAsyncResult ar)
        {
            tcpClient.Client.EndSend(ar);
        }

        public void SendUnReliable(byte[] buffer)
        {
            NetServer.Instance.SendUnReliable(RemoteEndPoint, buffer);
        }

        public void Close()
        {
            tcpClient.Close();
        }
    }
}
