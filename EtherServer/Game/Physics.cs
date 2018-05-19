using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public sealed class Physics
    {
        private static readonly Physics instance = new Physics();

        private Physics()
        {
        }

        public static Physics Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
