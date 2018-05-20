using System;
using System.Collections.Generic;
using System.Text;

using SVector3 = SharpNav.Geometry.Vector3;

namespace EtherServer.Game.Abilities
{
    class StrongSwordAttack : IAbility
    {
        public int Type()
        {
            return 1;
        }

        public void Use(int playerId, Vector3 target)
        {
            var position = World.Instance.players[playerId].position;
            var direction = ((SVector3)target - position).Normalized();
            foreach (var e in World.Instance.entities)
            {
                if(e.Value is Enemy)
                {
                    var enemy = e.Value as Enemy;
                    if (Hit(position, direction, enemy.Agent.Position))
                    {
                        enemy.Hit(60, this.Type(), playerId);
                        if (enemy.hp <= 0)
                            break;
                    }
                }
            }
        }

        bool Hit(SVector3 position, SVector3 direction, SVector3 enemyPosition)
        {
            if ((enemyPosition - position).Length() <= 3f)
            {
                var enDirection = (enemyPosition - position).Normalized();
                var dot = SVector3.Dot(direction, enDirection);

                if (dot >= Math.Sin(180 * 60 / Math.PI))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
