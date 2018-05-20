using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EtherServer.Game
{
    public class Artifact {
        public int typeId; // TypeID for classes of items
        public int uniqueId; // Unique ID of copy belonging to player
        public bool isReal; // False is player didn't pay for artifact transaction

        public Dictionary<string, int> effects;

        public Artifact(int typeId, int uniqueId, bool isReal){
            this.typeId = typeId;
            this.uniqueId = uniqueId;
            this.isReal = isReal;
            this.effects = new Dictionary<string, int>();
        }
    }
}