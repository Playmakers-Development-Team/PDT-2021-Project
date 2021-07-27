using System;
using Abilities;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Turn;
using Turn.Commands;
using Units;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using Units.Stats;
using UnityEngine;

namespace AI
{
    public class EnemyMeleeAi : MonoBehaviour
    {
        private EnemyUnit enemyUnit;
        
        private PlayerManager playerManager;
        private CommandManager commandManager;
        private TurnManager turnManager;
        private EnemyManager enemyManager;

        [SerializeField] private Ability meleeAttackAbility;
        [SerializeField] private Ability buffAbility;

        [SerializeField] private float actionDelay = 1.5f;
        [SerializeField] private int playerFleeDistance = 2;
        
        private void Awake()
        {
            enemyUnit = GetComponent<EnemyUnit>();
            
            playerManager = ManagerLocator.Get<PlayerManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            
            commandManager.ListenCommand<StartTurnCommand>(StartTurn);
        }
        
        private void StartTurn(StartTurnCommand startTurnCommand)
        {
            if(ReferenceEquals(startTurnCommand.Unit, enemyUnit))
                DecideEnemyIntention();
        }
        
        private async void DecideEnemyIntention()
        {
            if (playerManager.PlayerUnits.Count <= 0)
            {
                Debug.LogWarning("No players remain, enemy intention is to do nothing");
                return;
            }
            
            if (turnManager.TotalTurnCount % 2 == 0)
            {
                enemyManager.MoveToDistantTile(playerFleeDistance);
                
                commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
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
                    await enemyManager.MoveUnit(enemyUnit);
                
                    // If a player is now next to the enemy, attack the player
                    adjacentPlayerUnit = (IUnit) enemyManager.FindAdjacentPlayer(enemyUnit);

                    if (adjacentPlayerUnit != null)
                        await enemyManager.DoUnitAbility(enemyUnit, meleeAttackAbility, adjacentPlayerUnit);
                    else
                        await enemyManager.DoUnitAbility(enemyUnit, buffAbility, Vector2Int.zero);
                }

                commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
            }
        }
    }
}
