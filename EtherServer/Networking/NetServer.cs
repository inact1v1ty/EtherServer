using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EtherServer.Game;

namespace EtherServer.Networking
{
    public class NetServer
    {
        private static readonly NetServer instance = new NetServer();

        private NetServer() { }

        public static NetServer Instance
        {
            get
            {
                return instance;
            }
        }

        int port;

        ConcurrentDictionary<IPEndPoint, NetClient> clients;

        TcpListener listener;

        CancellationTokenSource cts;

        UdpClient udpClient;

        public void Init(int port)
        {
            this.port = port;
            cts = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            clients = new ConcurrentDictionary<IPEndPoint, NetClient>();
            udpClient = new UdpClient(port);
        }

        public void Run()
        {
            try
            {
                listener.Start();

                var task = AcceptClientsAsync(listener, cts.Token);
                if (task.IsFaulted)
                    task.Wait();

                udpClient.BeginReceive(OnUdpReceive, null);
            }
            finally
            {

            }
        }

        public void Stop()
        {
            try
            {
                cts.Cancel();
                listener.Stop();
                foreach (var client in clients.Values)
                {
                    client.Close();
                }
                clients.Clear();
            }
            finally
            {
            }
        }

        async Task AcceptClientsAsync(TcpListener listener, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                await AddClient(client, ct);
            }
        }

        async Task AddClient(TcpClient tcpClient, CancellationToken ct)
        {
            if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (tcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    return;
                }
            }

            var client = new NetClient(tcpClient);

            clients.AddOrUpdate(tcpClient.Client.RemoteEndPoint as IPEndPoint,
                client,
                (ip, cl) => { return client; });

            await World.Instance.AddPlayer(client);
        }

        public void ClientDisconnected(IPEndPoint endPoint)
        {
            World.Instance.ClientDisconnected(endPoint);
            if(clients.TryRemove(endPoint, out NetClient client)){ }
        }

        void OnUdpReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);

            try
            {
                byte[] data = udpClient.EndReceive(ar, ref endPoint);

                if (clients.TryGetValue(endPoint, out NetClient client))
                {
                    client.ReceivedUdp(data);
                }
                
                udpClient.BeginReceive(OnUdpReceive, null);
            }
            catch (SocketException ex)
            {
                ClientDisconnected(endPoint);
            }
            catch (ObjectDisposedException ex) { }
        }

        public void SendUnReliable(IPEndPoint endPoint, byte[] buffer)
        {
            try
            {
                udpClient.BeginSend(buffer, buffer.Length, endPoint, OnUdpSend, null);
            }
            catch (SocketException ex)
            {
                ClientDisconnected(endPoint);
            }
            catch (ObjectDisposedException ex) { }
        }

        void OnUdpSend(IAsyncResult ar)
        {
            udpClient.EndSend(ar);
        }

        public void OnProcessExit()
        {
            udpClient.Close();
            foreach (var c in clients)
            {
                c.Value.Close();
            }
        }
    }
}
