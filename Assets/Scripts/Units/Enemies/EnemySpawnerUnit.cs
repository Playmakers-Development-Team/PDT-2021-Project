using Abilities;
using TenetStatuses;
using UnityEngine;

namespace Units.Enemies
{
    public class EnemySpawnerUnit : EnemyUnit
    {
        [SerializeField] private int timer = 5;
        public GameObject SpawnPrefab;
        private EnemyUnit spawnUnit;

        public Vector2Int UnitPosition;

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
            if(--timer == 0)
            {
                Indestructible = false;
                UnitPosition = gridManager.ConvertPositionToCoordinate(transform.position);
                return true;
            }
            return false;
        }

        public override bool IsSameTeamWith(IAbilityUser other) => other is EnemyUnit;
    }
}
