using EtherServer.Networking;
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

        public NetClient client;

        public Vector3 position;

        public Inventory inventory;

        public Player(){
            inventory = new Inventory();
        }

        public async Task Init()
        {
            client.OnReliableReceived += OnReceived;
            client.OnUnReliableReceived += OnReceived;
            client.BeginTcpReceive();
            int artifacts = await this.inventory.getInventory(this);
            Console.WriteLine(artifacts);
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
                            World.Instance.UpdatePosition(this.id, position);
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
    }
}
