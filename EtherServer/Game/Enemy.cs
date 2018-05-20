using SharpNav;
using SharpNav.Crowds;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public class Enemy : Entity
    {
        public virtual int GameID()
        {
            return -1;
        }

        public virtual int DefaultHp()
        {
            return 0;
        }

        public AgentParams agentParams;

        public int agentID;

        public Agent Agent
        {
            get
            {
                return World.Instance.crowd.GetAgent(agentID);
            }
        }

        public int hp;

        public Enemy()
        {
            hp = DefaultHp();
            agentParams = new AgentParams();
            SetAgentParams();
            agentID = World.Instance.crowd.AddAgent(new Vector3(0, -4.72f, 0), agentParams);
        }

        public virtual void SetAgentParams() { }

        public void SetTargetPosition(Vector3 position)
        {
            NavMeshQuery q = new NavMeshQuery(World.Instance.navMesh, 2048);
            var poly = q.FindNearestPoly(position, new Vector3(5, 5, 5));
            SharpNav.Geometry.Vector3 v = SharpNav.Geometry.Vector3.Zero;
            q.ClosestPointOnPoly(poly.Polygon, position, ref v);
            Agent.RequestMoveTarget(poly.Polygon, v);
        }

        public override void Update()
        {
            
        }

        public virtual void Hit(int damage, int abilityType, int playerId)
        {
            hp -= damage;
            if (hp <= 0)
            {
                GiveLoot(ref World.Instance.players[playerId].inventory.bag);
                OnDeath();
            }
        }

        protected virtual void GiveLoot(ref List<Artifact> artifacts) { }
        protected virtual void OnDeath()
        {
            World.Instance.EnemyDied(this.id);
        }
    }
}
