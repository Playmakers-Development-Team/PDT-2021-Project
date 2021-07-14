using System;
using System.Collections.Generic;
using Abilities;
using Units.Commands;
using Units.Stats;
using TenetStatuses;
using UnityEngine;

namespace Units
{
    public interface IUnit : IDamageable, IKnockbackable, IAbilityUser
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        
        public string Name { get; set; }
        public TenetType Tenet { get; }
        public ValueStat MovementActionPoints { get; }
        public ValueStat Speed { get; }
        public ModifierStat Attack { get; }
        public List<Ability> Abilities { get; }

        public new Vector2Int Coordinate { get; }

        Sprite Render { get; }
        
        bool IsSelected { get; }
        Animator UnitAnimator { get; }

        void ChangeAnimation(AnimationStates animationStates);

        void TakeDamageWithoutModifiers(int amount);

        new void TakeDamage(int amount);

        new void TakeKnockback(int amount);

        new void TakeDefence(int amount);

        void SetSpeed(int amount);

        new void AddSpeed(int amount);
        
        void SetMovementActionPoints(int amount);
        
        new void TakeAttack(int amount);
        
        List<Vector2Int> GetAllReachableTiles();
        
        void MoveUnit(StartMoveCommand startMoveCommand);

        string RandomizeName();
    }
}
