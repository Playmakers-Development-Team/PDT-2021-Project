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

        public TenetType Tenet { get; }
        
        //TODO: Change this to a loadout type.
        public List<Ability> Abilities { get; }

        public new Vector2Int Coordinate { get; }

        Color UnitColor { get; }

        Animator UnitAnimator { get; }

        void ChangeAnimation(AnimationStates animationStates);

        void SetSpeed(int amount);

        new void AddSpeed(int amount);
        
        List<Vector2Int> GetAllReachableTiles();
        List<Vector2Int> GetReachableOccupiedTiles();

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

        /// <summary>
        /// <p>Upgrade an ability that is on existing unit. The ability will then be replaced with the
        /// upgraded ability.</p>
        ///
        /// <p>Will throw errors if the ability cannot be upgraded or you are trying to upgrade an ability
        /// that the unit does not have.</p>
        /// </summary>
        void UpgradeAbility(Ability existingAbility);

        /// <summary>
        /// <p>Upgrade an ability that is on existing unit. The ability will then be replaced with the
        /// upgraded ability.</p>
        ///
        /// <p>Will throw errors if the ability cannot be upgraded.</p>
        /// </summary>
        void UpgradeAbility(int index);
    }
}
