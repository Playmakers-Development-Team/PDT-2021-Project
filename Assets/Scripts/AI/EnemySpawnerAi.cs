using Abilities;
using Units;
using Units.Commands;
using Units.Enemies;
using UnityEngine;

namespace AI
{
    public class EnemySpawnerAi : EnemyAi
    {
        [SerializeField] private Ability meleeAttackAbility;
        [SerializeField] private Ability buffAbility;

        protected override async void DecideEnemyIntention()
        {
            if(enemyUnit is EnemySpawnerUnit enemySpawnerUnit)
            {
                if(enemySpawnerUnit.Turn()) enemyManager.Spawner(enemySpawnerUnit);
                else commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
            }
            else 
            {
                Debug.LogWarning("enemyspawnerai only works with enemyspawnerunit, dumbass");
                commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
            }
        }
    }
}
