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

        [SerializeField] private float actionDelay = 1.5f;
        [SerializeField] private int specialMoveCount = 3;
        [Tooltip("If set to 1, enemy will start retreating on the second round, if set to 0 on the first round")]
        [SerializeField] private int specialMoveOffset = 1;
        
        protected override async void DecideEnemyIntention()
        {
            if (playerManager.PlayerUnits.Count <= 0)
            {
                Debug.LogWarning("No players remain, enemy intention is to do nothing");
                return;
            }

            if (specialMoveCount != 0 &&
                (turnManager.RoundCount + specialMoveOffset) % specialMoveCount == 0)
            {
                await enemyManager.MoveToDistantTile(enemyUnit);
            }
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
                        await enemyManager.DoUnitAbility(enemyUnit, meleeAttackAbility,
                            adjacentPlayerUnit);
                    else if (buffAbility != null)
                        await enemyManager.DoUnitAbility(enemyUnit, buffAbility,
                            Vector2Int.zero);
                }
            }
            
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }
    }
}
