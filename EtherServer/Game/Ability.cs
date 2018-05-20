using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public interface IAbility
    {
        int Type();
        void Use(int playerId, Vector3 target);
    }

    public static class AbilitiesList
    {
        public static IAbility Get(int type)
        {
            return abilities[type];
        }
        static Dictionary<int, IAbility> abilities = new Dictionary<int, IAbility>()
        {
            {1, new Abilities.StrongSwordAttack() }
        };
    }
}
