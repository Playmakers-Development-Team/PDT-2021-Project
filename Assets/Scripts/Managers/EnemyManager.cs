using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : Manager
    {
        private readonly List<IUnit> enemyUnits = new List<IUnit>();

        public IReadOnlyList<IUnit> EnemyUnits => enemyUnits.AsReadOnly();
        public int Count => enemyUnits.Count;
        
        public IUnit Spawn(GameObject enemyPrefab, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(enemyPrefab, gridPosition);
            
            if (!(unit is EnemyUnit))
                return null;
            
            enemyUnits.Add(unit);
            
            return unit;
        }
        
        public IUnit Spawn(string enemyName, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(enemyName, gridPosition);

            if (!(unit is EnemyUnit))
                return null;
            
            enemyUnits.Add(unit);
            
            return unit;
        }
        
        public void Clear()
        {
            enemyUnits.Clear();
        }

        public void Clean()
        {
            for (int i = enemyUnits.Count; i >= 0; i--)
            {
                if (enemyUnits[i] is null)
                    enemyUnits.RemoveAt(i);
            }
        }
    }
}
