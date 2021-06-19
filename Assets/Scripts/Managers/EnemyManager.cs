using System;
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
        
        public void MoveUnit(EnemyUnit actingUnit)
        {
            // TODO: Find position to move to
            //Vector2Int gridPos;
            
            IUnit enemyUnit = actingUnit;
            IUnit closestPlayerUnit = FindClosestPlayer(actingUnit);
            Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
                      " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            // var moveCommand = new MoveCommand(
            //     enemyUnit,
            //     gridPos,
            //     enemyUnit.Coordinate
            // );
            
            // commandManager.ExecuteCommand(moveCommand);
        }

        // TODO: Find a way to account for obstacles that may be in the way
        public IUnit FindClosestPlayer(IUnit enemyUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            
            IUnit closestPlayerUnit = playerManager.PlayerUnits[0];
            int closestPlayerUnitDistance = Int32.MaxValue;

            foreach (var playerUnit in playerManager.PlayerUnits)
            {
                int xDistance = Mathf.Abs(playerUnit.Coordinate.x - enemyUnit.Coordinate.x);
                int yDistance = Mathf.Abs(playerUnit.Coordinate.y - enemyUnit.Coordinate.y);

                // If a new closest unit is found, assign a new closest unit
                if (closestPlayerUnitDistance > xDistance + yDistance)
                {
                    closestPlayerUnitDistance = xDistance + yDistance;
                    closestPlayerUnit = playerUnit;
                }
                // If the closest distances are the same, find the lower health unit
                else if (closestPlayerUnitDistance == xDistance + yDistance)
                {
                    // If both unit health values are the same then the closestPlayerUnit is kept the same 
                    // i.e. The earlier player in the list is prioritised
                    if (closestPlayerUnit.Health.HealthPoints.Value != playerUnit.Health.HealthPoints.Value)
                    {
                        float lowerHealth = Mathf.Min(closestPlayerUnit.Health.HealthPoints.Value, playerUnit.Health.HealthPoints.Value);
                        if (lowerHealth == playerUnit.Health.HealthPoints.Value)
                        {
                            closestPlayerUnit = playerUnit;
                        }
                    }
                }
            }

            return closestPlayerUnit;
        }
        
        // IsPlayerAdjacent will return true as soon as it finds a player adjacent to the given gridObject
        // otherwise will return false
        public GridObject FindAdjacentPlayer(GridObject gridObject)
        {
            Vector2Int gridObjectPosition = gridObject.Coordinate;
            
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
