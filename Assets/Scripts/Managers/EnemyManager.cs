using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commands;
using Cysharp.Threading.Tasks;
using GridObjects;
using Units;
using Units.Commands;
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
        
        private TurnManager turnManager;
        private GridManager gridManager;
        private PlayerManager playerManager;

        public override void ManagerStart()
        {
            base.ManagerStart();
            
            turnManager = ManagerLocator.Get<TurnManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        /// <summary>
        /// Spawns in an enemy unit and adds it the the <c>enemyUnits</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c> that was added.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit unit = base.Spawn(unitPrefab, gridPosition);
            enemyUnits.Add(unit);
            commandManager.ExecuteCommand(new SpawnedUnitCommand(unit));
            return unit;
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
        
        public void RemoveUnit(IUnit targetUnit) => enemyUnits.Remove(targetUnit);

        public async void DecideEnemyIntention(EnemyUnit enemyUnit)
        {
            IUnit adjacentPlayerUnit = (IUnit) FindAdjacentPlayer(enemyUnit);

            if (adjacentPlayerUnit != null)
            {
                await AttackUnit(enemyUnit, adjacentPlayerUnit);
            }
            else if (playerManager.PlayerUnits.Count > 0)
            {
                Debug.Log(enemyUnit.Name + " ENEMY-INT: Move towards player");
                await MoveUnit(enemyUnit);
                
                while (playerManager.WaitForDeath)
                    await UniTask.Yield();
                
                // If a player is now next to the enemy, attack the player
                adjacentPlayerUnit = (IUnit) FindAdjacentPlayer(enemyUnit);
                if (adjacentPlayerUnit != null)
                {
                    await AttackUnit(enemyUnit, adjacentPlayerUnit);
                }
            }
            else
            {
                Debug.LogWarning("WARNING: No players remain, enemy intention is to do nothing");
                return;
            }
            
            commandManager.ExecuteCommand(new EndTurnCommand(turnManager.ActingUnit));
        }

        private async Task AttackUnit(EnemyUnit enemyUnit, IUnit playerUnit)
        {
            // TODO: Will later need to be turned into an ability command when enemies have abilities
            Debug.Log(enemyUnit.Name + " ENEMY-INT: Damage player");
            playerUnit.TakeDamage((int) enemyUnit.Attack.Modify(1));
            
            await UniTask.Delay(1000); // just so that an enemies turn does not instantly occ

            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        private UniTask MoveUnit(EnemyUnit enemyUnit)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);
			
            // Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
            //           " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            var moveCommand = new StartMoveCommand(
                enemyUnit,
                enemyUnit.Coordinate + FindClosestPath(enemyUnit, targetPlayerUnit, (int) 
                enemyUnit.MovementActionPoints.Value)
            );
            
            ManagerLocator.Get<CommandManager>().ExecuteCommand(moveCommand);
            commandManager.ExecuteCommand(moveCommand);
            return UniTask.Delay(1000);
        }
        
        private Vector2Int FindClosestPath(EnemyUnit enemyUnit, IUnit targetUnit, int movementPoints)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            Vector2Int movementDir = Vector2Int.zero;

            List<Vector2Int> targetTiles = GetAdjacentFreeSquares(targetUnit);
            Vector2Int chosenTargetTile = Vector2Int.zero; // PLACEHOLDER INITIALISATION VALUE
            
            List<Vector2Int> reachableTiles =
                gridManager.GetAllReachableTiles(enemyUnit.Coordinate, movementPoints);

            if (targetTiles.Count == 0)
            {
                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy is stationary until a new tile is available adjacent to the target player " + targetUnit);
                return Vector2Int.zero;
            }
            
            chosenTargetTile = gridManager.GetClosestCoordinateFromList(targetTiles, enemyUnit.Coordinate);
            movementDir = gridManager.GetClosestCoordinateFromList(reachableTiles, chosenTargetTile) - enemyUnit.Coordinate;

            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy to move in the direction " + movementDir + " towards target tile " + chosenTargetTile + ". Player is at " + targetUnit.Coordinate);
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
                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Targeting closest player " + targetPlayerUnit);
            }
            else if (closestPlayersCount > 1)
            {
                List<IUnit> lowestHealthPlayers = GetLowestHealthPlayers(closestPlayers);
				
                // If 1 low HP player is returned, it is set as the target player unit
                // If multiple low HP players are returned, the first instance is set as the target
                targetPlayerUnit = lowestHealthPlayers[0];
                
                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Targeting lower HP player " + targetPlayerUnit + 
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
                // Remove target coordinate is out of bounds
                if (gridManager.GetTileDataByCoordinate(adjacentCoordinates[i]) == null)
                {
                    adjacentCoordinates.RemoveAt(i);
                }
                // Remove if target coordinate is occupied
                else if (gridManager.GetGridObjectsByCoordinate(adjacentCoordinates[i]).Count > 0)
                {
                    adjacentCoordinates.RemoveAt(i);
                }
            }

            // NOTE: If no nearby player squares are free, an empty list is returned
            return adjacentCoordinates;
        }
    }
}
