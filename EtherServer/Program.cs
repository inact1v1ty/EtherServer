using System;
using System.Threading;
using System.Threading.Tasks;
using EtherServer.Game;
using EtherServer.Networking;

namespace EtherServer
{
    class Program
    {
        static void Main(string[] args)
        {
            World.Instance.Init();
            var loopTask = Task.Factory.StartNew(World.Instance.Run);
            NetServer.Instance.Init(7777);
            NetServer.Instance.Run();
            Console.ReadKey();
        }
    }
}