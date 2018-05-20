using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game.Enemies
{
    class TestBoss : Enemy
    {
        public override void SetAgentParams()
        {
            agentParams.Height = 2f;
            agentParams.Radius = 1f;
            agentParams.MaxSpeed = 0.1f;
        }
        public override void Update()
        {
            base.Update();
            var agent = World.Instance.crowd.GetAgent(agentID);
            var position = agent.Position;
            int minp = -1;
            float mind = float.MaxValue;
            foreach (var p in World.Instance.players)
            {
                if ((position - p.Value.position).Length() < mind)
                {
                    mind = (position - p.Value.position).Length();
                    minp = p.Key;
                }
            }
            if (minp != -1)
                agent.TargetPosition = World.Instance.players[minp].position;
        }
    }
}
