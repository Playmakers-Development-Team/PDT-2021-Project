using System.Collections.Generic;
using Abilities;
using Units.Commands;
using Units.Stats;
using TenetStatuses;
using UnityEngine;

namespace Units
{
    public interface IUnit : IStat, IAbilityUser
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        public string Name { get; set; }
        public TenetType Tenet { get; }
        
        public Stat MovementPoints { get; }
        
        public Stat AttackStat { get; }
        public Stat DefenceStat { get; }
        public Stat SpeedStat { get; }

        public Stat KnockbackStat { get; }
        
        //TODO: Change this to a loadout type.
        public List<Ability> Abilities { get; }

        public new Vector2Int Coordinate { get; }

        Color UnitColor { get; }

        Animator UnitAnimator { get; }

        void ChangeAnimation(AnimationStates animationStates);

        void SetSpeed(int amount);

        new void AddSpeed(int amount);
        
        List<Vector2Int> GetAllReachableTiles();

        void MoveUnit(StartMoveCommand startMoveCommand);

        string RandomizeName();

        void SetTenets(ITenetBearer tenetBearer);
    }
}
