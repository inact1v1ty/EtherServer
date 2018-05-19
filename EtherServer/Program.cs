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
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            World.Instance.Init();
            var loopTask = Task.Factory.StartNew(World.Instance.Run);
            NetServer.Instance.Init(7777);
            NetServer.Instance.Run();
            Console.ReadKey();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            NetServer.Instance.OnProcessExit();
        }
    }
}