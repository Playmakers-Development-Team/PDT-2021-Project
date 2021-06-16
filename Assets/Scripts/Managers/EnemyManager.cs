using System;
using System.Collections.Generic;
using Commands;
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

        public void DecideEnemyIntention(EnemyUnit actingUnit)
        {
            if (FindAdjacentPlayer(actingUnit))
            {
                Debug.Log("Implement enemy damage command here");
            }
            else
            {
                MoveUnit(actingUnit);
            }
        }
        
        public void MoveUnit(EnemyUnit actingUnit)
        {
            IUnit enemyUnit = actingUnit;
            IUnit closestPlayerUnit = FindClosestPlayer(actingUnit);
            // Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
            //           " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            var moveCommand = new MoveCommand(
                enemyUnit,
                enemyUnit.Coordinate + FindClosestPath(actingUnit, closestPlayerUnit, (int) actingUnit.Speed.Value),
                enemyUnit.Coordinate
            );
            
            ManagerLocator.Get<CommandManager>().ExecuteCommand(moveCommand);
        }

        // This is a super basic movement system. Enemies will not go into occupied tiles
        // but aren't smart enough to path-find around occupied tiles to get to players
        // TODO: Find a way to account for obstacles that may be in the way
        private Vector2Int FindClosestPath(EnemyUnit actingUnit, IUnit targetUnit, float movementPoints)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            Vector2Int movementDir = Vector2Int.zero;
            
            for (int i = 0; i < movementPoints; ++i)
            {
                List<GridObject> adjacentGridObjects = GetAdjacentGridObjects(actingUnit.Coordinate + movementDir);

                foreach (var adjacentGridObject in adjacentGridObjects)
                {
                    // If a player is adjacent, break out the function
                    if (adjacentGridObject.CompareTag("PlayerUnit"))
                    {
                        Debug.Log("RETURNED ZERO");
                        return Vector2Int.zero;
                    }
                }
                
                if (actingUnit.Coordinate.x != targetUnit.Coordinate.x)
                {
                    int xDir = (int) Mathf.Sign(targetUnit.Coordinate.x - actingUnit.Coordinate.x + movementDir.x);
                    
                    // Check that the tile isn't occupied
                    if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                        (actingUnit.Coordinate.x + xDir, actingUnit.Coordinate.y)).Count == 0)
                    {
                        movementDir = movementDir + Vector2Int.right * xDir;
                    }
                }
                else if (actingUnit.Coordinate.y != targetUnit.Coordinate.y)
                {
                    int yDir = (int) Mathf.Sign(targetUnit.Coordinate.y - actingUnit.Coordinate.y + movementDir.y);
                    
                    //Check that the tile isn't occupied
                    if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                        (actingUnit.Coordinate.x, actingUnit.Coordinate.y + yDir)).Count == 0)
                    {
                        movementDir = movementDir + Vector2Int.up * yDir;
                    }
                }
            }
            Debug.Log(actingUnit.Coordinate + " | " + movementDir);
            return movementDir;
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
        public GridObject FindAdjacentPlayer(IUnit enemyUnit)
        {
            List<GridObject> adjacentGridObjects = GetAdjacentGridObjects(enemyUnit.Coordinate);

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                {
                    return adjacentGridObject;
                }
            }
            
            return null;
        }

        public List<GridObject> GetAdjacentGridObjects(Vector2Int enemyUnitCoordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(enemyUnitCoordinate + Vector2Int.up));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(enemyUnitCoordinate + Vector2Int.right));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(enemyUnitCoordinate + Vector2Int.down));
            adjacentGridObjects.AddRange(gridManager.GetGridObjectsByCoordinate(enemyUnitCoordinate + Vector2Int.left));

            return adjacentGridObjects;
        }
    }
}
