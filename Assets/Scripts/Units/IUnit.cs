using System;
using Abilities;
using UnityEngine;

namespace Units
{
    public interface IUnit
    {
        Type GetDataType();
        
        void Damage(int amount);

        void Defend(int amount);

        void Knockback(Vector2Int translation);

        int GetStacks(Tenet tenet);

        void Expend(Tenet tenet, int amount);
    }
}
