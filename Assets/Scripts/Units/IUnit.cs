using System;
using System.Collections.Generic;
using GridObjects;
using Abilities;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public interface IUnit
    {
        public ValueStat HealthPoints { get; }
        public ValueStat MovementActionPoints { get; }
        public ModifierStat DealDamageModifier { get; }
        public ModifierStat TakeDamageModifier { get; }

        public ValueStat Speed { get; }
        
        public Vector2Int Coordinate { get; }
        IEnumerable<TenetStatusEffect> TenetStatusEffects { get; }

        Type GetDataType();
        
        void TakeDamage(int amount);

        void TakeDefence(int amount);
        
        void TakeAttack(int amount);

        void Knockback(Vector2Int translation);

        List<Ability> GetAbilities();

        void AddOrReplaceTenetStatusEffect(TenetType tenetType, int stackCount = 1);

        bool RemoveTenetStatusEffect(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatusEffects();

        public int GetTenetStatusEffectCount(TenetType tenetType);

        bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1);

        bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatusEffect tenetStatusEffect);

        bool IsActing();

        bool IsSelected();
    }
}
