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

        // This is a super basic movement system. Enemies will not go into occupied tiles
        // but aren't smart enough to path-find around occupied tiles to get to players
        // TODO: Find a way to account for obstacles that may be in the way
        private Vector2Int FindClosestPath(EnemyUnit actingUnit, IUnit targetUnit, float movementPoints)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            Vector2Int targetUnitCoordinate = FindClosestAdjacentFreeSquare(actingUnit, targetUnit);
            
            Vector2Int movementDir = Vector2Int.zero;
            
            for (int i = 0; i < movementPoints; ++i)
            {
                List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(actingUnit.Coordinate + movementDir);

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
                            playerUnit.TakeDamage((int) actingUnit.DealDamageModifier.Value);
                        }
                        return movementDir;
                    }
                }

                int newMovementX = 0;
                int newMovementY = 0;
                // Check if x coordinate is not the same as target
                if (actingUnit.Coordinate.x != targetUnitCoordinate.x)
                {
                    newMovementX = (int) Mathf.Sign(targetUnitCoordinate.x - actingUnit.Coordinate.x - movementDir.x);
                }
                // Check if y coordinate is not the same as target
                if (actingUnit.Coordinate.y != targetUnitCoordinate.y)
                {
                   newMovementY = (int) Mathf.Sign(targetUnitCoordinate.y - actingUnit.Coordinate.y - movementDir.y);
                }

                if (newMovementX != 0 && TryMoveX(actingUnit, movementDir, newMovementX))
                {
                    movementDir = movementDir + Vector2Int.right * newMovementX;
                }
                else if(newMovementY != 0 && TryMoveY(actingUnit, movementDir, newMovementY)) // If moving on the X fails, try move on the Y
                {
                    movementDir = movementDir + Vector2Int.up * newMovementY;
                }
            }

            return movementDir;
        }

        private bool TryMoveX(EnemyUnit actingUnit, Vector2Int previousMovement, int newMovementX)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            // Check that the tile isn't occupied
            if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                (actingUnit.Coordinate.x + previousMovement.x + newMovementX,
                actingUnit.Coordinate.y + previousMovement.y)).Count == 0)
            {
                return true;
            }

            return false;
        }

        private bool TryMoveY(EnemyUnit actingUnit, Vector2Int previousMovement, int newMovementY)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
                
            //Check that the tile isn't occupied
            if (gridManager.GetGridObjectsByCoordinate(new Vector2Int
                (actingUnit.Coordinate.x + previousMovement.x,
                actingUnit.Coordinate.y + previousMovement.y + newMovementY)).Count == 0)
            {
                return true;
            }

            return false;
        }

        // TODO: Find a way to account for obstacles that may be in the way
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
            List<IUnit> closestPlayerUnits = new List<IUnit>();
            int closestPlayerUnitDistance = Int32.MaxValue;
            
            foreach (var playerUnit in playerUnits)
            {
                // TODO: This is to be changed to use Lachlan's Breadth-First search
                int xDistance = Mathf.Abs(playerUnit.Coordinate.x - enemyUnit.Coordinate.x);
                int yDistance = Mathf.Abs(playerUnit.Coordinate.y - enemyUnit.Coordinate.y);
            
                // If a new closest unit is found, assign a new closest unit
                if (closestPlayerUnitDistance > xDistance + yDistance)
                {
                    closestPlayerUnits.Clear();
                    closestPlayerUnitDistance = xDistance + yDistance;
                    closestPlayerUnits.Add(playerUnit);
                }
                else if (closestPlayerUnitDistance == xDistance + yDistance)
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

        private Vector2Int FindClosestAdjacentFreeSquare(EnemyUnit actingUnit, IUnit targetUnit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            
            Dictionary<Vector2Int, float> coordinateDistances = new Dictionary<Vector2Int, float>();

            Vector2Int northCoordinate = targetUnit.Coordinate + Vector2Int.up;
            Vector2Int eastCoordinate = targetUnit.Coordinate + Vector2Int.right;
            Vector2Int southCoordinate = targetUnit.Coordinate + Vector2Int.down;
            Vector2Int westCoordinate = targetUnit.Coordinate + Vector2Int.left;
            
            coordinateDistances.Add(northCoordinate, Vector2.Distance(northCoordinate, actingUnit.Coordinate));
            coordinateDistances.Add(eastCoordinate, Vector2.Distance(eastCoordinate, actingUnit.Coordinate));
            coordinateDistances.Add(southCoordinate, Vector2.Distance(southCoordinate, actingUnit.Coordinate));
            coordinateDistances.Add(westCoordinate, Vector2.Distance(westCoordinate, actingUnit.Coordinate));

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
