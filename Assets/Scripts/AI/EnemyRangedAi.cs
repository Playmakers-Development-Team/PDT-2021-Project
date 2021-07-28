using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities;
using Units;
using Units.Commands;
using UnityEngine;
using Utilities;

namespace AI
{
    public class EnemyRangedAi : EnemyAi
    {
        [SerializeField] private int safeDistanceRange = 2;
        private int shootingRange;

        [SerializeField] private Ability rangedAttackAbility;
        [SerializeField] private Ability secondRangedAttackAbility;
        [SerializeField] private Ability buffAbility;
        
        protected override async void DecideEnemyIntention()
        {
            shootingRange = 3;
            
            if (playerManager.PlayerUnits.Count <= 0)
            {
                Debug.LogWarning("No players remain, enemy intention is to do nothing");
                return;
            }
            
            if (turnManager.RoundCount % SpecialMoveCount == 0) //EVEN TURNS
            {
                if (ArePlayersClose())
                {
                    await enemyManager.MoveToDistantTile(enemyUnit);
                    
                    if (GetTargetsInRange().Count > 0)
                        await ShootPlayer(secondRangedAttackAbility);
                }
                else
                {
                    if (GetTargetsInRange().Count > 0)
                        await ShootPlayer(secondRangedAttackAbility);
                }
            }
            else //ODD TURNS
            {
                await enemyManager.MoveToTargetRange(enemyUnit, shootingRange);

                if (GetTargetsInRange().Count > 0)
                    await ShootPlayer(rangedAttackAbility);
                else
                    await enemyManager.DoUnitAbility(enemyUnit, buffAbility, Vector2Int.zero);
            }
            
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }

        /// <summary>
        /// Returns true if a player is within <c>safeDistanceRange</c> tiles of the enemy.
        /// Assumes that obstacles are not a factor
        /// </summary>
        private bool ArePlayersClose()
        {
            foreach (var playerUnit in playerManager.PlayerUnits)
            {
                if (safeDistanceRange <= ManhattanDistance.GetManhattanDistance(
                    playerUnit.Coordinate, enemyUnit.Coordinate))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Returns all players within <c>shootingRange</c> tiles of the enemy.
        /// Assumes that all obstacles can be shot through
        /// </summary>
        private List<IUnit> GetTargetsInRange()
        {
            List<IUnit> targetsInRange = new List<IUnit>();
            
            foreach (var playerUnit in playerManager.PlayerUnits)
            {
                if (shootingRange <= ManhattanDistance.GetManhattanDistance(
                    playerUnit.Coordinate, enemyUnit.Coordinate))
                    targetsInRange.Add(playerUnit);
            }
            
            return targetsInRange;
        }
        
        /// <summary>
        /// Returns a player within shooting range. If there are multiple players, the player
        /// with the lowest HP is chosen
        /// </summary>
        private IUnit GetTargetUnit()
        {
            if(GetTargetsInRange().Count <= 0)
            {
                Debug.LogWarning("EnemyRangedAi is trying to get a target player but" +
                                 "no players are close enough. Avoid calling this function");
                return null;
            }

            return enemyManager.GetLowestHealthPlayers(GetTargetsInRange())[0];
        }
        
        /// <summary>
        /// Returns true if a player is within <c>shootingRange</c> tiles of the enemy.
        /// Assumes that all obstacles can be shot through
        /// </summary>
        private async Task ShootPlayer(Ability abilityType)
        {
            await enemyManager.DoUnitAbility(enemyUnit, abilityType, GetTargetUnit());
        }
    }
}
