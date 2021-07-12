using System;
using System.Collections.Generic;
using Abilities;
using Units.Commands;
using Units.Stats;
using Units.TenetStatuses;
using UnityEngine;

namespace Units
{
    public interface IUnit : IDamageable, IKnockbackable
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        
        public string Name { get; set; }
        public TenetType Tenet { get; }
        public ValueStat MovementActionPoints { get; }
        public ValueStat Speed { get; }
        public ModifierStat Attack { get; }
        public List<Ability> Abilities { get; }

        public Vector2Int Coordinate { get; }
        
        [Obsolete("Use TenetStatuses instead")]
        ICollection<TenetStatus> TenetStatusEffects { get; }
        ICollection<TenetStatus> TenetStatuses { get; }
        
        Sprite Render { get; }
        
        bool IsSelected { get; }
        Animator UnitAnimator { get; }

        void ChangeAnimation(AnimationStates animationStates);

        void TakeDamage(int amount);

        void TakeKnockback(int amount);

        void TakeDefence(int amount);

        void SetSpeed(int amount);
        
        void SetMovementActionPoints(int amount);
        
        void TakeAttack(int amount);
        
        List<Vector2Int> GetAllReachableTiles();
        
        void MoveUnit(StartMoveCommand startMoveCommand);

        void AddOrReplaceTenetStatus(TenetType tenetType, int stackCount = 1);

        bool RemoveTenetStatus(TenetType tenetType, int amount = int.MaxValue);

        void ClearAllTenetStatus();

        [Obsolete("Use GetTenetStatusCount instead")]
        public int GetTenetStatusEffectCount(TenetType tenetType);
        
        public int GetTenetStatusCount(TenetType tenetType);

        [Obsolete("Use HasTenetStatus instead")]
        bool HasTenetStatusEffect(TenetType tenetType, int minimumStackCount = 1);
        
        bool HasTenetStatus(TenetType tenetType, int minimumStackCount = 1);

        [Obsolete("Use TryGetTenetStatus instead")]
        bool TryGetTenetStatus(TenetType tenetType, out TenetStatus tenetStatus);
        
        bool TryGetTenetStatusEffect(TenetType tenetType, out TenetStatus tenetStatus);

        string RandomizeName();
    }
}
