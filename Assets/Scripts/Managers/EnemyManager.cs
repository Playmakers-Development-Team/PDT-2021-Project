using System;
using System.Collections.Generic;
using Commands;
using GridObjects;
using Units;
using UnityEditor.Timeline.Actions;
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
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            
            IUnit adjacentPlayerUnit = (IUnit) FindAdjacentPlayer(actingUnit);
            
            if (adjacentPlayerUnit != null)
            {
                // TODO: Will later need to be turned into an ability command when enemies have abilities
                Debug.Log("ENEMY-INT: Damage player");
                adjacentPlayerUnit.TakeDamage((int) actingUnit.DealDamageModifier.Value);
            }
            else if (playerManager.PlayerUnits.Count > 0)
            {
                Debug.Log("ENEMY-INT: Move towards player");
                MoveUnit(actingUnit);
            }
            else
            {
                Debug.Log("ENEMY-INT: Do nothing (No players)");
            }
        }

        private void MoveUnit(EnemyUnit actingUnit)
        {
            IUnit enemyUnit = actingUnit;
            IUnit targetPlayerUnit = GetTargetPlayer(actingUnit);
            // Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
            //           " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            var moveCommand = new MoveCommand(
                enemyUnit,
                enemyUnit.Coordinate + FindClosestPath(actingUnit, targetPlayerUnit, (int) actingUnit.MovementActionPoints.Value)
            );
            
            ManagerLocator.Get<CommandManager>().ExecuteCommand(moveCommand);
        }
        
        private Vector2Int FindClosestPath(EnemyUnit actingUnit, IUnit targetUnit, int movementPoints)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            Vector2Int movementDir = Vector2Int.zero;

            int shortestDistance = int.MaxValue;

            List<Vector2Int> reachableTiles =
                gridManager.GetAllReachableTiles(actingUnit.Coordinate, movementPoints);

            List<Vector2Int> targetTiles = GetAdjacentFreeSquares(targetUnit);

            if (targetTiles.Count == 0)
            {
                Debug.LogWarning("ENEMY-TAR: Enemy is stationary until a new tile is available adjacent to the target player " + targetUnit);
                return Vector2Int.zero;
            }
            
            foreach (var reachableTile in reachableTiles)
            {
                Dictionary<Vector2Int, int> distanceToAllCells =
                    gridManager.GetDistanceToAllCells(reachableTile);
                
                foreach (var targetTile in targetTiles)
                {
                    if (distanceToAllCells[targetTile] < shortestDistance)
                    {
                        shortestDistance = distanceToAllCells[targetTile];
                        movementDir = reachableTile - actingUnit.Coordinate;
                    }
                }
            }

            Debug.Log("ENEMY-TAR: Enemy to move in the direction " + movementDir);
            return movementDir;
        }

        public IUnit GetTargetPlayer(IUnit enemyUnit)
        {
            IUnit targetPlayerUnit;
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();

            List<IUnit> closestPlayers = GetClosestPlayers(enemyUnit, playerManager.PlayerUnits);
            int closestPlayersCount = closestPlayers.Count;

            if (closestPlayersCount == 1)
            {
                targetPlayerUnit = closestPlayers[0];
                Debug.Log("ENEMY-TAR: Targeting closest player " + targetPlayerUnit);
            }
            else if (closestPlayersCount > 1)
            {
                List<IUnit> lowestHealthPlayers = GetLowestHealthPlayers(closestPlayers);

                // If 1 low HP player is returned, it is set as the target player unit
                // If multiple low HP players are returned, the first instance is set as the target
                targetPlayerUnit = lowestHealthPlayers[0];
                
                Debug.Log("ENEMY-TAR: Targeting lower HP player " + targetPlayerUnit + 
                          "(Multiple closest players found)");
            }
            else
            {
                Debug.LogWarning("WARNING: GetTargetPlayer() called but no players remain in" +
                                 "PlayerManager.PlayerUnits. Please avoid calling this function");
                return null;
            }

            return targetPlayerUnit;
        }

        private List<IUnit> GetClosestPlayers(IUnit enemyUnit, IReadOnlyList<IUnit> playerUnits)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            Dictionary<Vector2Int, int> distanceToAllCells = gridManager.GetDistanceToAllCells(enemyUnit.Coordinate);
            
            List<IUnit> closestPlayerUnits = new List<IUnit>();
            int closestPlayerUnitDistance = Int32.MaxValue;
            
            foreach (var playerUnit in playerUnits)
            {
                int playerDistanceToEnemy = distanceToAllCells[playerUnit.Coordinate];

                // If a new closest unit is found, assign a new closest unit
                if (closestPlayerUnitDistance > playerDistanceToEnemy)
                {
                    closestPlayerUnits.Clear();
                    closestPlayerUnitDistance = playerDistanceToEnemy;
                    closestPlayerUnits.Add(playerUnit);
                }
                else if (closestPlayerUnitDistance == playerDistanceToEnemy)
                {
                    closestPlayerUnits.Add(playerUnit);
                }
            }

            return closestPlayerUnits;
        }

        private List<IUnit> GetLowestHealthPlayers(IReadOnlyList<IUnit> playerUnits)
        {
            List<IUnit> lowestHealthPlayerUnits = new List<IUnit>();
            float lowestHealthValue = Int32.MaxValue;
            
            foreach (var playerUnit in playerUnits)
            {
                if (lowestHealthValue > playerUnit.Health.HealthPoints.Value)
                {
                    lowestHealthPlayerUnits.Clear();
                    lowestHealthValue = playerUnit.Health.HealthPoints.Value;
                    lowestHealthPlayerUnits.Add(playerUnit);
                }
                else if (lowestHealthValue == playerUnit.Health.HealthPoints.Value)
                {
                    lowestHealthPlayerUnits.Add(playerUnit);
                }
            }

            return lowestHealthPlayerUnits;
        }

        // Repurposed this function from "FindClosestAdjacentFreeSquare" to "GetAdjacentFreeSquares"
        private List<Vector2Int> GetAdjacentFreeSquares(IUnit targetUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            List<Vector2Int> adjacentCoordinates = new List<Vector2Int>();

            adjacentCoordinates.Add(targetUnit.Coordinate + Vector2Int.up);
            adjacentCoordinates.Add(targetUnit.Coordinate + Vector2Int.right);
            adjacentCoordinates.Add(targetUnit.Coordinate + Vector2Int.down);
            adjacentCoordinates.Add(targetUnit.Coordinate + Vector2Int.left);

            for (int i = adjacentCoordinates.Count - 1; i > -1; i--)
            {
                if (gridManager.GetGridObjectsByCoordinate(adjacentCoordinates[i]).Count > 0)
                {
                    //Debug.Log("Removing coord as it's not free " + adjacentCoordinates[i]);
                    adjacentCoordinates.RemoveAt(i);
                }
            }

            // NOTE: If no nearby player squares are free, an empty list is returned
            return adjacentCoordinates;
        }
        
        public GridObject FindAdjacentPlayer(IUnit enemyUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(enemyUnit.Coordinate);

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
