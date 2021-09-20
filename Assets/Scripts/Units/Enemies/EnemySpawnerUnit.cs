using System.Threading.Tasks;
using Abilities;
using Managers;
using TenetStatuses;
using Units.Commands;
using UnityEngine;

namespace Units.Enemies
{
    public class EnemySpawnerUnit : EnemyUnit
    {
        [SerializeField] private int timer = 5;
        public GameObject SpawnPrefab;
        private EnemyUnit spawnUnit;

        private void Start()
        {            
            spawnUnit = SpawnPrefab.GetComponent<EnemyUnit>();  
            HealthStat.BaseValue = Mathf.FloorToInt((float) spawnUnit.GetBaseHealth() / 2);
            HealthStat.Reset();
            AddOrReplaceTenetStatus(TenetType.Pride, timer); //temp?
        }

        public bool Turn()
        {
            RemoveTenetStatus(TenetType.Pride, 1); //temp?
            if (--timer == 0)
            {
                Indestructible = false;
                return true;
            }
            return false;
        }

        public override bool IsSameTeamWith(IAbilityUser other) => other is EnemyUnit;
        
        public async Task Spawner()
        {
            // Get spawner stats
            int damage = HealthStat.BaseValue - HealthStat.Value;
            int curSpeed = SpeedStat.Value;

            // Kill spawner
            TakeDamage(HealthStat.Value + DefenceStat.Value + 20);
            await commandManager.WaitForCommand<KilledUnitCommand>();

            // Spawn unit
            GameObject spawnPrefab = SpawnPrefab;
            spawnPrefab.GetComponent<EnemyUnit>().HealthStat.BaseValue = 5;
            var spawnedUnit = unitManagerT.Spawn(spawnPrefab, Coordinate);
            spawnedUnit.SetSpeed(curSpeed - 1);
            spawnedUnit.TakeDamage(damage);
            await commandManager.WaitForCommand<SpawnedUnitCommand>(); //IMPORTANT
            unitManagerT.AddUnit(spawnedUnit);
            // Apply spawner stats  
            // BUG: Setting speed too late, needs to be set before spawning so that the unit can be
            // BUG: placed correctly on the timeline.

        }
    }
}
