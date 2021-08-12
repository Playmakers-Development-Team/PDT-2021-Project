using System.Collections.Generic;
using Abilities;
using Abilities.Commands;
using Abilities.Shapes;
using Units.Commands;
using Units.Stats;
using TenetStatuses;
using Units.Virtual;
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

        /// <summary>
        /// <p>Use an ability</p>
        ///
        /// <p>This will use the <c>AbilityCommand</c></p>
        /// </summary>
        /// <param name="ability">The Ability to be used</param>
        /// <param name="direction">Direction to use to ability towards</param>
        /// <returns><c>AbilityCommand</c> which might be helpful later</returns>
        AbilityCommand UseAbility(Ability ability, ShapeDirection direction);

        /// <summary>
        /// <p>Tries to use an ability and simulate the result of its effects</p>
        ///
        /// <p>Note: This function only catches affected IUnits ONLY</p>
        /// </summary>
        /// <param name="ability">The Ability to be used</param>
        /// <param name="direction">Direction to use to ability towards</param>
        /// <returns>All units that are affected</returns>
        IEnumerable<VirtualUnit> ProjectAbility(Ability ability, ShapeDirection direction);
    }
}
