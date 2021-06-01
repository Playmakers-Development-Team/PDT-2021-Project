using System.Collections.Generic;
using GridObjects;
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
            return Spawn(UnitUtility.Spawn(enemyPrefab, gridPosition));
        }
        
        public IUnit Spawn(string enemyName, Vector2Int gridPosition)
        {
            return Spawn(UnitUtility.Spawn(enemyName, gridPosition));
        }

        private IUnit Spawn(IUnit unit)
        {
            if (!(unit is EnemyUnit))
                return null;
            
            enemyUnits.Add(unit);
            
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);

            return unit;
        } 
        
        public void Clear()
        {
            enemyUnits.Clear();
        }

        public void ClearUnits()
        {
            for (int i = enemyUnits.Count; i >= 0; i--)
            {
                if (enemyUnits[i] is null)
                    enemyUnits.RemoveAt(i);
            }
        }
        
        public void RemoveEnemyUnit(IUnit enemyUnit)
        {
            if (enemyUnits.Contains(enemyUnit))
            {
                enemyUnits.Remove(enemyUnit);
                Debug.Log(enemyUnits.Count + " enemies remain");
            }
            else
            {
                Debug.LogWarning("WARNING: Tried to remove " + enemyUnit +
                                 " from EnemyManager but it isn't a part of the enemyUnits list");
            }
        }
        
        // IsPlayerAdjacent will return true as soon as it finds a player adjacent to the given gridObject
        // otherwise will return false
        public GridObject FindAdjacentPlayer(GridObject gridObject)
        {
            Vector2Int gridObjectPosition = gridObject.GetCoordinate();
            
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.up));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.right));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.down));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(gridObjectPosition + Vector2Int.left));

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                {
                    return adjacentGridObject;
                }
            }
            
            return null;
        }
    }
}
