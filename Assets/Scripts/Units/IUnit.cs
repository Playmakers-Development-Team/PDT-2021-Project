using System;
using System.Collections.Generic;
using GridObjects;
using Abilities;
using StatusEffects;
using UnityEngine;

namespace Units
{
    public interface IUnit : IDamageable, IKnockbackable
    {
        public string Name { get; set; }
        public TenetType Tenet { get; }
        public ValueStat MovementActionPoints { get; }
        public ValueStat Speed { get; }
        public ModifierStat Attack { get; }
        public List<Ability> Abilities { get; }

        public Vector2Int Coordinate { get; }

        GameObject gameObject { get; }

        IEnumerable<TenetStatusEffect> TenetStatusEffects { get; }

        Sprite Render { get; }

        Type GetDataType();

        void TakeDamage(int amount);

        void TakeKnockback(int amount);

        void TakeDefence(int amount);

        void TakeAttack(int amount);

        void AddOrReplaceTenetStatusEffect(TenetType tenetType, int stackCount = 1);

        bool RemoveTenetStatusEffect(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatusEffects();

        public int GetTenetStatusEffectCount(TenetType tenetType);

        bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1);

        bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatusEffect tenetStatusEffect);

        bool IsSelected();

        string RandomizeName();
    }
}
