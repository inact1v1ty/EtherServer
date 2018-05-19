using System;
using EtherServer.Game;
using EtherServer.Networking;

namespace EtherServer
{
    class Program
    {
        static void Main(string[] args)
        {
            World.Instance.Init();
            NetServer.Instance.Init(7777);
            NetServer.Instance.Run();
            Console.ReadKey();
        }
    }
}