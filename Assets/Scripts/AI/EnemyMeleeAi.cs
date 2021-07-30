using Abilities;
using Units;
using Units.Commands;
using UnityEngine;

namespace AI
{
    public class EnemyMeleeAi : EnemyAi
    {
        [SerializeField] private Ability meleeAttackAbility;
        [SerializeField] private Ability buffAbility;

        protected override async void DecideEnemyIntention()
        {
            if (playerManager.Units.Count <= 0)
            {
                Debug.LogWarning("No players remain, enemy intention is to do nothing");
                return;
            }
            
            if (turnManager.RoundCount % SpecialMoveCount == 0)
                await enemyManager.MoveToDistantTile(enemyUnit);
            else
            {
                IUnit adjacentPlayerUnit = (IUnit) enemyManager.FindAdjacentPlayer(enemyUnit);

                if (adjacentPlayerUnit != null)
                {
                    await enemyManager.DoUnitAbility(enemyUnit, meleeAttackAbility,
                        adjacentPlayerUnit);
                }
                else
                {
                    await enemyManager.MoveUnitToTarget(enemyUnit);
                
                    // If a player is now next to the enemy, attack the player
                    adjacentPlayerUnit = (IUnit) enemyManager.FindAdjacentPlayer(enemyUnit);

                    if (adjacentPlayerUnit != null)
                        await enemyManager.DoUnitAbility(enemyUnit, meleeAttackAbility, adjacentPlayerUnit);
                    else
                        await enemyManager.DoUnitAbility(enemyUnit, buffAbility, Vector2Int.zero);
                }
            }
            
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }
    }
}
