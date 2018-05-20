using SharpNav.Crowds;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public class Enemy : Entity
    {
        public AgentParams agentParams;
        public int agentID;
        public Enemy()
        {
            agentParams = new AgentParams();
            SetAgentParams();
            agentID = World.Instance.crowd.AddAgent(new Vector3(3f, -4.72f, 65.35f), agentParams);
        }
        public virtual void SetAgentParams()
        {
            
        }
        public override void Update()
        {
            
        }
    }
}
