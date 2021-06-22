using System;
using System.Collections.Generic;
using Commands;
using GridObjects;
using Units;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : UnitManager
    {
        /// <summary>
        /// Holds all the enemy units currently in the level.
        /// </summary>
        private readonly List<IUnit> enemyUnits = new List<IUnit>();

        /// <summary>
        /// Returns all enemy units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> EnemyUnits => enemyUnits.AsReadOnly();

        /// <summary>
        /// Clears all the enemies from the <c>enemyUnits</c> list.
        /// </summary>
        public void ClearEnemyUnits() => enemyUnits.Clear();

        /// <summary>
        /// Spawns in an enemy unit and adds it the the <c>enemyUnits</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c> that was added.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
            enemyUnits.Add(newUnit);
            return newUnit;
        }

        public GridObject FindAdjacentPlayer(IUnit enemyUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(enemyUnit.Coordinate);

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                    return adjacentGridObject;
            }

            return null;
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

        public void DecideEnemyIntention(EnemyUnit unit)
        {
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            
            IUnit adjacentPlayerUnit = (IUnit) FindAdjacentPlayer(unit);
            
            if (adjacentPlayerUnit != null)
            {
                // TODO: Will later need to be turned into an ability command when enemies have abilities
                adjacentPlayerUnit.TakeDamage((int) unit.DealDamageModifier.Value);
            }
            else if (playerManager.PlayerUnits.Count > 0)
            {
                MoveUnit(unit);
            }
            else
            {
                Debug.LogWarning("WARNING: No players remain, enemy intention is to do nothing");
            }
        }

        private void MoveUnit(EnemyUnit unit)
        {
            IUnit enemyUnit = unit;
            IUnit closestPlayerUnit = FindClosestPlayer(unit);
            // Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
            //           " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            var moveCommand = new MoveCommand(
                enemyUnit,
                enemyUnit.Coordinate + FindClosestPath(unit, closestPlayerUnit, (int) unit.MovementActionPoints.Value)
            );
            
            ManagerLocator.Get<CommandManager>().ExecuteCommand(moveCommand);
        }

        // This is a super basic movement system. Enemies will not go into occupied tiles
        // but aren't smart enough to path-find around occupied tiles to get to players
        // TODO: Find a way to account for obstacles that may be in the way
        private Vector2Int FindClosestPath(EnemyUnit unit, IUnit targetUnit, float movementPoints)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Vector2Int targetUnitCoordinate = FindClosestAdjacentFreeSquare(unit, targetUnit);
            
            Vector2Int movementDir = Vector2Int.zero;
            
            for (int i = 0; i < movementPoints; ++i)
            {
                List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(unit.Coordinate + movementDir);

                foreach (var adjacentGridObject in adjacentGridObjects)
                {
                    // If a player is adjacent, break out the function
                    if (adjacentGridObject.CompareTag("PlayerUnit"))
                    {
                        // If there are still available movement points, do an attack
                        if (i + 1 < movementPoints)
                        {
                            IUnit playerUnit = (IUnit) adjacentGridObject;
                            
                            // TODO: Will later need to be turned into an ability command when enemies have abilities
                            playerUnit.TakeDamage((int) unit.DealDamageModifier.Value);
                        }
                        return movementDir;
                    }
                }

                int newMovementX = 0;
                int newMovementY = 0;
                // Check if x coordinate is not the same as target
                if (unit.Coordinate.x != targetUnitCoordinate.x)
                {
                    newMovementX = (int) Mathf.Sign(targetUnitCoordinate.x - unit.Coordinate.x - movementDir.x);
                }
                // Check if y coordinate is not the same as target
                if (unit.Coordinate.y != targetUnitCoordinate.y)
                {
                   newMovementY = (int) Mathf.Sign(targetUnitCoordinate.y - unit.Coordinate.y - movementDir.y);
                }

                if (newMovementX != 0 && TryMoveX(unit, movementDir, newMovementX))
                {
                    movementDir = movementDir + Vector2Int.right * newMovementX;
                }
                else if(newMovementY != 0 && TryMoveY(unit, movementDir, newMovementY)) // If moving on the X fails, try move on the Y
                {
                    movementDir = movementDir + Vector2Int.up * newMovementY;
                }
            }

            return movementDir;
        }

        private bool TryMoveX(EnemyUnit unit, Vector2Int previousMovement, int newMovementX)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            // Check that the tile isn't occupied
            if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                (unit.Coordinate.x + previousMovement.x + newMovementX,
                unit.Coordinate.y + previousMovement.y)).Count == 0)
            {
                return true;
            }

            return false;
        }

        private bool TryMoveY(EnemyUnit unit, Vector2Int previousMovement, int newMovementY)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
                
            //Check that the tile isn't occupied
            if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                (unit.Coordinate.x + previousMovement.x,
                unit.Coordinate.y + previousMovement.y + newMovementY)).Count == 0)
            {
                return true;
            }

            return false;
        }
        
        // TODO: Find a way to account for obstacles that may be in the way
        public IUnit FindClosestPlayer(IUnit enemyUnit)
        {
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
        
        private Vector2Int FindClosestAdjacentFreeSquare(EnemyUnit unit, IUnit targetUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            Dictionary<Vector2Int, float> coordinateDistances = new Dictionary<Vector2Int, float>();
            
            Vector2Int northCoordinate = targetUnit.Coordinate + Vector2Int.up;
            Vector2Int eastCoordinate = targetUnit.Coordinate + Vector2Int.right;
            Vector2Int southCoordinate = targetUnit.Coordinate + Vector2Int.down;
            Vector2Int westCoordinate = targetUnit.Coordinate + Vector2Int.left;
            
            coordinateDistances.Add(northCoordinate, Vector2.Distance(northCoordinate, unit.Coordinate));
            coordinateDistances.Add(eastCoordinate, Vector2.Distance(eastCoordinate, unit.Coordinate));
            coordinateDistances.Add(southCoordinate, Vector2.Distance(southCoordinate, unit.Coordinate));
            coordinateDistances.Add(westCoordinate, Vector2.Distance(westCoordinate, unit.Coordinate));

            Vector2Int closestCoordinate = targetUnit.Coordinate;
            float closestDistance = float.MaxValue; // Placeholder initialisation value
            

            foreach (var coordinateDistance in coordinateDistances)
            {
                // Is the distance close AND is the tile unoccupied
                if (coordinateDistance.Value < closestDistance &&
                    gridManager.GetGridObjectsByCoordinate(coordinateDistance.Key).Count == 0)
                {
                    closestCoordinate = coordinateDistance.Key;
                }
            }
            
            // NOTE: If no nearby player squares are free, targetUnit.Coordinate is returned
            return closestCoordinate;
        }
    }
}
