using Units.Commands;
using Units.Enemies;
using UnityEngine;

namespace AI
{
    public class EnemySpawnerAi : EnemyAi
    {
        protected override async void DecideEnemyIntention()
        {
            // TODO: Why is this check not being done in the other AI classes?
            if (!(enemyUnit is EnemySpawnerUnit enemySpawnerUnit))
                Debug.LogWarning("enemyspawnerai only works with enemyspawnerunit, dumbass");
            else
            {
                if (enemySpawnerUnit.Turn())
                    await enemySpawnerUnit.Spawner();
            }

            // TODO: Move to superclass.
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }
    }
}
