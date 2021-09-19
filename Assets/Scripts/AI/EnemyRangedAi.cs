using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abilities;
using Abilities.Shapes;
using Cysharp.Threading.Tasks;
using Units;
using Units.Commands;
using Units.Players;
using UnityEngine;
using Utilities;

namespace AI
{
    public class EnemyRangedAi : EnemyAi
    {
        [SerializeField] private int safeDistanceRange = 2;

        [SerializeField] private Ability rangedAttackAbility;
        [SerializeField] private Ability secondRangedAttackAbility;
        [SerializeField] private Ability buffAbility;

        [Header("Additional Options")]
        [SerializeField] private bool onlyMoveInOneAxis;
        
        protected override async UniTask DecideEnemyIntention()
        {
            if (turnManager.RoundCount + 1 % SpecialMoveCount == 0) //EVEN TURNS
            {
                if (ArePlayersClose())
                {
                    await enemyManager.MoveToDistantTile(enemyUnit);
                    
                    if (GetTargetsInRange(secondRangedAttackAbility).Count > 0)
                        await ShootPlayer(secondRangedAttackAbility);
                }
                else
                {
                    if (GetTargetsInRange(secondRangedAttackAbility).Count > 0)
                        await ShootPlayer(secondRangedAttackAbility);
                }
            }
            else //ODD TURNS
            {
                await MoveToTarget();

                if (GetTargetsInRange(rangedAttackAbility).Count > 0)
                    await ShootPlayer(rangedAttackAbility);
                else
                    await enemyManager.DoUnitAbility(enemyUnit, buffAbility, ShapeDirection.None);
            }
        }

        /// <summary>
        /// Move closer towards a target
        /// </summary>
        private async UniTask MoveToTarget()
        {
            if (onlyMoveInOneAxis)
            {
                List<Vector2Int> allowedTiles = enemyUnit.GetReachableOccupiedTiles()
                    .Where(coor => coor.x == enemyUnit.Coordinate.x || coor.y == enemyUnit.Coordinate.y)
                    .ToList();

                await enemyManager.MoveToTargetRange(enemyUnit, safeDistanceRange, allowedTiles);
            }
            else
            {
                await enemyManager.MoveToTargetRange(enemyUnit, safeDistanceRange);
            }
        }

        /// <summary>
        /// Returns true if a player is within <c>safeDistanceRange</c> tiles of the enemy.
        /// Assumes that obstacles are not a factor
        /// </summary>
        private bool ArePlayersClose()
        {
            foreach (var playerUnit in playerManager.Units)
            {
                if (safeDistanceRange <= ManhattanDistance.GetManhattanDistance(
                    playerUnit.Coordinate, enemyUnit.Coordinate))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns all players within <c>shootingRange</c> tiles of the enemy.
        /// Assumes that all obstacles cannot be shot through for now
        /// </summary>
        private List<IUnit> GetTargetsInRange(Ability abilityType) => abilityType.Shape
            .GetTargetsInAllDirections(enemyUnit.Coordinate)
            .OfType<PlayerUnit>()
            .OfType<IUnit>()
            .ToList();

        /// <summary>
        /// Returns a player within shooting range. If there are multiple players, the player
        /// with the lowest HP is chosen
        /// </summary>
        private IUnit GetTargetUnit(Ability abilityType)
        {
            if(GetTargetsInRange(abilityType).Count <= 0)
            {
                Debug.LogWarning("EnemyRangedAi is trying to get a target player but" +
                                 "no players are close enough. Avoid calling this function");
                return null;
            }

            return enemyManager.GetLowestHealthPlayers(GetTargetsInRange(abilityType))[0];
        }
        
        /// <summary>
        /// Returns true if a player is within <c>shootingRange</c> tiles of the enemy.
        /// Assumes that all obstacles cannot be shot through for now
        /// </summary>
        private async UniTask ShootPlayer(Ability abilityType)
        {
            await enemyManager.DoUnitAbility(enemyUnit, abilityType, GetTargetUnit(abilityType));
        }
    }
}
