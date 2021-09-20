using Cysharp.Threading.Tasks;
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
            ///TODO BUG: PLEASE DO NOT REMOVE THIS, FOR THE LOVE OF GOD THE GAME WILL DIE DONT REMOVE THIS CODE AT 3AM PLEASE
            await UniTask.Yield(); /// <----------------------------------------------------------------------------------
          

            
            // TODO: Move to superclass.
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }
    }
}
