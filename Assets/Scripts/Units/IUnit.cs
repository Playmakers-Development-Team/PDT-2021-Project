using System;
using System.Collections.Generic;
using Abilities;
using Units.Commands;
using Units.Stats;
using TenetStatuses;
using UnityEngine;

namespace Units
{
    //TODO: Remove IDamageable and IKnockbackable reference.
    public interface IUnit : IDamageable, IKnockbackable, IStat, IAbilityUser
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        
        public string Name { get; set; }
        public TenetType Tenet { get; }
        
        // [Obsolete("Use MovementPoints instead")]
        // public ValueStat MovementActionPoints { get; }
        public Stat MovementPoints { get; }
        
        [Obsolete("Use SpeedStat instead ")] 
        public ValueStat Speed { get; }
        
        [Obsolete ("Use AttackStat instead")]
        public ModifierStat Attack { get; }
        public Stat AttackStat { get; }
        public Stat DefenceStat { get; }
        public Stat SpeedStat { get; }

        public Stat KnockbackStat { get; }
        
        //TODO: Change this to a loadout type.
        public List<Ability> Abilities { get; }

        public new Vector2Int Coordinate { get; }

        Sprite Render { get; }
        
        bool IsSelected { get; }
        Animator UnitAnimator { get; }

        void ChangeAnimation(AnimationStates animationStates);

        void SetSpeed(int amount);
        
        void SetMovementActionPoints(int amount);

        [Obsolete]
        void TakeDamageWithoutModifiers(int amount);
        
        List<Vector2Int> GetAllReachableTiles();
        
        void MoveUnit(StartMoveCommand startMoveCommand);

        string RandomizeName();
    }
}
