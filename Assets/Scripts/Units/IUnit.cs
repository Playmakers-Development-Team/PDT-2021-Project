using System;
using GridObjects;
using Abilities;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public interface IUnit
    {
        public ModifierStat DealDamageModifier { get; }
        
        public ValueStat Speed { get; }
        
        public Vector2Int Coordinate { get; }
        
        Type GetDataType();
        
        void TakeDamage(int amount);

        void AddDefence(int amount);
        
        void AddAttack(int amount);

        void Knockback(Vector2Int translation);

        void AddOrReplaceTenetStatusEffect(TenetType tenetType, int stackCount = 1);

        bool RemoveTenetStatusEffect(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatusEffects();

        public int GetTenetStatusEffectCount(TenetType tenetType);

        bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1);

        bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatusEffect tenetStatusEffect);
    }
}
