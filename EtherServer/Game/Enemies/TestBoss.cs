using SharpNav;
using System;
using System.Collections.Generic;
using System.Text;

using SVector3 = SharpNav.Geometry.Vector3;

namespace EtherServer.Game.Enemies
{
    class TestBoss : Enemy
    {
        int targetPlayer = -1;
        public override int GameID()
        {
            return 1;
        }
        public override int DefaultHp()
        {
            return 100;
        }
        public TestBoss()
        {
            agentParams = new SharpNav.Crowds.AgentParams();
            SetAgentParams();
            var tpos = new SharpNav.Geometry.Vector3(3f, -4.62f, 65.35f);
            World.Instance.crowd.GetAgent(agentID).Position = tpos;
        }
        public override void SetAgentParams()
        {
            agentParams.Height = 2f;
            agentParams.Radius = 1f;
            agentParams.MaxSpeed = 3f;
            agentParams.MaxAcceleration = 8f;
            agentParams.UpdateFlags = (SharpNav.Crowds.UpdateFlags)31;
        }
        public override void Update()
        {
            base.Update();

            
            if (targetPlayer != -1)
            {
                if (SVector3.Subtract(Agent.Position, World.Instance.players[targetPlayer].position).Length() < 1.5f)
                {
                    SetTargetPosition(Agent.Position);
                }
                else
                {
                    SetTargetPosition(World.Instance.players[targetPlayer].position);
                }
            } else
            {
                GetTarget();
            }
        }

        void GetTarget()
        {
            var position = Agent.Position;
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
                targetPlayer = minp;
        }

        protected override void GiveLoot(ref List<Artifact> artifacts)
        {
            artifacts.Add(Artifact.CreateInWorld(0));
        }
    }
}
