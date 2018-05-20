using EtherServer.Networking;
using SharpNav.Crowds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EtherServer.Game
{
    public class Player
    {
        public int id;

        public string Name { get; set; }

        public int agentID;

        public NetClient client;

        public Vector3 position;

        public Inventory inventory;

        public string address;

        public Player(){
            inventory = new Inventory();
        }

        public async Task Init()
        {
            var agentParams = new AgentParams();
            agentParams.Height = 1.75f;
            agentParams.Radius = 0.4f;
            agentParams.UpdateFlags = (UpdateFlags)(0);
            agentParams.MaxSpeed = 0;
            agentID = World.Instance.crowd.AddAgent(new SharpNav.Geometry.Vector3(3.02f, -4.72f, 65.34f), agentParams);
            client.OnReliableReceived += OnReceived;
            client.OnUnReliableReceived += OnReceived;
            client.BeginTcpReceive();
            int artifacts = await this.inventory.getInventory(this);
        }

        private void OnReceived(byte[] buffer, int length)
        {
            using (MemoryStream ms = new MemoryStream(buffer, 0, length))
            using (BinaryReader br = new BinaryReader(ms))
            {
                var netMessage = (NetMessage)(br.ReadInt32());

                switch (netMessage)
                {
                    case NetMessage.Nickname:
                        {
                            int count = br.ReadInt32();
                            var bytes = br.ReadBytes(count);
                            Name = Encoding.UTF8.GetString(bytes);
                            World.Instance.UpdateNickname(this.id, Name);

                            count = br.ReadInt32();
                            bytes = br.ReadBytes(count);
                            this.address = Encoding.UTF8.GetString(bytes);
                        }
                        break;
                    case NetMessage.GetPlayers:
                        {
                            var players = World.Instance.GetPlayers(this.id);
                            using (MemoryStream ms2 = new MemoryStream())
                            using (BinaryWriter bw = new BinaryWriter(ms2))
                            {
                                bw.Write((int)(NetMessage.GetPlayers));
                                bw.Write(players.Count);
                                for (int i = 0; i < players.Count; i++)
                                {
                                    bw.Write(players[i].id);
                                    var bytes = Encoding.UTF8.GetBytes(players[i].nickname);
                                    bw.Write(bytes.Length);
                                    bw.Write(bytes);
                                    bw.Write(players[i].position);
                                }
                                client.SendReliable(ms2.ToArray());
                            }
                        }
                        break;
                    case NetMessage.UpdatePosition:
                        {
                            var position = br.ReadVector3();
                            this.position = position;
                            World.Instance.crowd.GetAgent(agentID).Position = position;
                            World.Instance.UpdatePosition(this.id, position);
                        }
                        break;
                    case NetMessage.GetEnemies:
                        {
                            var enemies = World.Instance.GetEnemies();
                            using (MemoryStream ms2 = new MemoryStream())
                            using (BinaryWriter bw = new BinaryWriter(ms2))
                            {
                                bw.Write((int)(NetMessage.GetEnemies));
                                bw.Write(enemies.Count);
                                for (int i = 0; i < enemies.Count; i++)
                                {
                                    bw.Write(enemies[i].id);
                                    bw.Write(enemies[i].gameID);
                                    bw.Write(enemies[i].position);
                                }
                                client.SendReliable(ms2.ToArray());
                            }
                        }
                        break;
                }
            }
        }

        public void PlayerConnected(int id)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.PlayerConnected));
                bw.Write(id);
                client.SendReliable(ms.ToArray());
            }
        }

        public void PlayerDisconnected(int id)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.PlayerDisconnected));
                bw.Write(id);
                client.SendReliable(ms.ToArray());
            }
        }

        public void NicknameUpdate(int id, string nickname)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.Nickname));
                bw.Write(id);
                var bytes = Encoding.UTF8.GetBytes(nickname);
                bw.Write(bytes.Length);
                bw.Write(bytes);
                client.SendReliable(ms.ToArray());
            }
        }

        public void UpdatePosition(int id, Vector3 position)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.UpdatePosition));
                bw.Write(id);
                bw.Write(position);
                client.SendUnReliable(ms.ToArray());
            }
        }

        public void SpawnEnemy(int id, int gameID)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.SpawnEnemy));
                bw.Write(id);
                bw.Write(gameID);
                client.SendReliable(ms.ToArray());
            }
        }

        public void UpdateEnemyPosition(int id, Vector3 position)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.UpdateEnemyPosition));
                bw.Write(id);
                bw.Write(position);
                client.SendUnReliable(ms.ToArray());
            }
        }
    }
}
