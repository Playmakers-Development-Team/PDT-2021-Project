using System;
using GridObjects;
using Abilities;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public interface IUnit
    {
        public Stat DealDamageModifier { get; }
        
        Type GetDataType();
        
        void Damage(int amount);

        void Defend(int amount);

        void Knockback(Vector2Int translation);

        int GetStacks(TenetType tenetType);

        void Expend(TenetType tenetType, int amount);
    }
}
