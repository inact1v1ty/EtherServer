using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EtherServer.Networking;

namespace EtherServer.Game
{
    public sealed class World
    {
        private static readonly World instance = new World();

        public static World Instance
        {
            get
            {
                return instance;
            }
        }

        HashSet<int> usedIds;

        Dictionary<int, Player> players;

        CancellationTokenSource cts;

        Stopwatch stopwatch;

        List<Entity> entities;

        private World()
        {
            players = new Dictionary<int, Player>();
            usedIds = new HashSet<int>();
            cts = new CancellationTokenSource();
            stopwatch = new Stopwatch();
            entities = new List<Entity>();
        }

        public void Init()
        {

        }

        public void Run()
        {
            var ct = cts.Token;

            while (!ct.IsCancellationRequested)
            {
                stopwatch.Restart();
                Update();
                stopwatch.Stop();
                var delay = TimeSpan.FromMilliseconds(2000) - stopwatch.Elapsed;
                if (delay.Ticks > 0) {
                    Thread.Sleep(delay);
                }
            }
        }

        void Update()
        {
            foreach(var e in entities)
            {
                e.Update();
            }
        }

        public async Task AddPlayer(NetClient client)
        {
            var player = new Player
            {
                client = client,
                id = GenerateId()
            };
            usedIds.Add(player.id);
            players.Add(player.id, player);
            await player.Init();

            foreach (var p in players)
            {
                if (p.Key != player.id)
                {
                    p.Value.PlayerConnected(player.id);
                }
            }
            Console.WriteLine("Player from {0} connected", client.RemoteEndPoint);
        }

        public void UpdateNickname(int id, string nickname)
        {
            foreach (var p in players)
            {
                if (p.Key != id)
                {
                    p.Value.NicknameUpdate(id, nickname);
                }
            }
        }

        public List<(int id, string nickname, Vector3 position)> GetPlayers(int id)
        {
            List<(int id, string nickname, Vector3 position)> list = new List<(int id, string nickname, Vector3 position)>();
            foreach (var p in players)
            {
                if (p.Key != id)
                {
                    list.Add((p.Value.id, p.Value.Name, p.Value.position));
                }
            }

            return list;
        }

        public void UpdatePosition(int id, Vector3 position)
        {
            foreach (var p in players)
            {
                if (p.Key != id)
                {
                    p.Value.UpdatePosition(id, position);
                }
            }
        }

        public void ClientDisconnected(IPEndPoint endPoint)
        {
            int id = -1;
            foreach (var p in players)
            {
                if (p.Value.client.RemoteEndPoint == endPoint)
                {
                    id = p.Value.id;
                }
            }
            if (id != -1) {
                foreach (var p in players)
                {
                    if (p.Key != id)
                    {
                        p.Value.PlayerDisconnected(id);
                    }
                }
                players.Remove(id);
                Console.WriteLine("Player from {0} disconnected", endPoint);
            }
        }

        int GenerateId()
        {
            var random = new Random();
            int id;
            do
            {
                id = random.Next();
            } while (usedIds.Contains(id));
            return id;
        }
    }
}
