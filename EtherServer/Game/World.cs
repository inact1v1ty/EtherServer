using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpNav;
using EtherServer.Networking;
using SharpNav.Geometry;
using SharpNav.Crowds;

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

        HashSet<int> entityIds;

        public Dictionary<int, Player> players;

        CancellationTokenSource cts;

        Stopwatch stopwatch;

        Dictionary<int, Entity> entities;

        public TiledNavMesh navMesh;

        public Crowd crowd;

        private World()
        {
            players = new Dictionary<int, Player>();
            usedIds = new HashSet<int>();
            entityIds = new HashSet<int>();
            cts = new CancellationTokenSource();
            stopwatch = new Stopwatch();
            entities = new Dictionary<int, Entity>();
            navMesh = NavMeshHelper.GetNavMesh() as TiledNavMesh;
            crowd = new Crowd(256, 1f, ref navMesh);
        }

        public void Init()
        {
            Console.WriteLine("Boss incoming");
            var boss = new Enemies.TestBoss();
            AddEntity(boss);
        }

        public void Run()
        {
            var ct = cts.Token;

            while (!ct.IsCancellationRequested)
            {
                stopwatch.Restart();
                Update();
                stopwatch.Stop();
                var delay = TimeSpan.FromMilliseconds(20) - stopwatch.Elapsed;
                if (delay.Ticks > 0) {
                    Thread.Sleep(delay);
                }
            }
        }

        void Update()
        {
            crowd.Update(0.02f);
            foreach(var e in entities)
            {
                e.Value.Update();
                if (e.Value is Enemy)
                {
                    var enemy = e.Value as Enemy;
                    UpdateEnemyPosition(e.Key, crowd.GetAgent(enemy.agentID).Position);
                }
            }
        }

        public async Task AddPlayer(NetClient client)
        {
            var player = new Player
            {
                client = client,
                id = GenerateId(usedIds)
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

        public void AddEntity(Entity entity)
        {
            var id = GenerateId(entityIds);
            entity.id = id;
            entities.Add(id, entity);
            var gameID = (entity as Enemy).GameID();
            foreach (var p in players)
            {
                p.Value.SpawnEnemy(id, gameID);
            }
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

        public List<(int id, int gameID, Vector3 position)> GetEnemies()
        {
            var list = new List<(int id, int gameID, Vector3 position)>();
            foreach (var e in entities)
            {
                if (e.Value is Enemy)
                {
                    var en = e.Value as Enemy;
                    list.Add((e.Key, en.GameID(), en.Agent.Position));
                }
            }

            return list;
        }

        void UpdateEnemyPosition(int id, Vector3 position)
        {
            foreach (var p in players)
            {
                p.Value.UpdateEnemyPosition(id, position);
            }
        }

        int GenerateId(HashSet<int> set)
        {
            var random = new Random();
            int id;
            do
            {
                id = random.Next();
            } while (set.Contains(id));
            return id;
        }
    }
}
