using Units.Commands;
using Units.Enemies;
using UnityEngine;

namespace AI
{
    public class EnemySpawnerAi : EnemyAi
    {
        protected override async void DecideEnemyIntention()
        {
            if(enemyUnit is EnemySpawnerUnit enemySpawnerUnit)
            {
                if(enemySpawnerUnit.Turn()) enemyManager.Spawner(enemySpawnerUnit);
                // TODO: Move to superclass. Always executed.
                else commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
            }
            else 
            {
                Debug.LogWarning("enemyspawnerai only works with enemyspawnerunit, dumbass");
                // TODO: Move to superclass. Always executed.
                commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
            }
        }
    }
}
