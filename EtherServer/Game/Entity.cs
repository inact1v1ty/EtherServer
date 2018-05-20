using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public class Entity
    {
        public int id;
        public virtual void Update()
        {
            Console.WriteLine("Entity");
        }
    }
}
