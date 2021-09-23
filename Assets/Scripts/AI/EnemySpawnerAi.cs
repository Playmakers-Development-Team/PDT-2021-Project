using Cysharp.Threading.Tasks;
using Units.Commands;
using Units.Enemies;
using UnityEngine;

namespace AI
{
    public class EnemySpawnerAi : EnemyAi
    {
        protected override async UniTask DecideEnemyIntention()
        {
            // TODO: Why is this check not being done in the other AI classes?
            if (!(enemyUnit is EnemySpawnerUnit enemySpawnerUnit))
                Debug.LogWarning("enemyspawnerai only works with enemyspawnerunit, dumbass");
            else
            {
                if (enemySpawnerUnit.Turn())
                    await enemySpawnerUnit.Spawner();
            }
        }
    }
}
