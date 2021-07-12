using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commands;
using Cysharp.Threading.Tasks;
using GridObjects;
using Managers;
using Units.Commands;
using Units.Players;
using UnityEngine;

namespace Units.Enemies
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
        /// Adds a unit to the <c>enemyUnits</c> list.
        /// </summary>
        public void AddUnit(IUnit targetUnit) => enemyUnits.Add(targetUnit);
        
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

        // TODO: Will later need to be turned into an ability command when enemies have abilities
        private async Task AttackUnit(EnemyUnit enemyUnit, IUnit playerUnit)
        {
            // TODO: The EnemyAttack command can be deleted once enemy abilities are implemented
            commandManager.ExecuteCommand(new EnemyAttack(enemyUnit));
            
            Debug.Log(enemyUnit.Name + " ENEMY-INT: Damage player");
            playerUnit.TakeDamage((int) enemyUnit.Attack.Modify(1));
            
            // Wait so that an enemies turn is not over instantly
            await UniTask.Delay(1000); 
            
            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        private async Task MoveUnit(EnemyUnit enemyUnit)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);
			
            // Debug.Log("Closest player to " + enemyUnit + " at " + enemyUnit.Coordinate + 
            //           " is " + closestPlayerUnit + " at " + closestPlayerUnit.Coordinate);

            var moveCommand = new StartMoveCommand(
                enemyUnit,
                FindClosestPath(enemyUnit, targetPlayerUnit, (int) 
                enemyUnit.MovementActionPoints.Value)
            );
            
            commandManager.ExecuteCommand(moveCommand);
            await UniTask.Delay(3500);
        }
        
        private Vector2Int FindClosestPath(EnemyUnit enemyUnit, IUnit targetUnit, int movementPoints)
        {
            //TODO: Find out why negative movement points are being passed in
            if (movementPoints <= 0)
            {
                Debug.Log(enemyUnit.Name +
                          " ENEMY-TAR: Enemy is stationary as it has no movement points");
                return Vector2Int.zero;
            }
            
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            // Can uncomment if we want enemies to flank to free adjacent squares
            // List<Vector2Int> targetTiles = gridManager.GetAdjacentFreeSquares(targetUnit);

            List<Vector2Int> reachableTiles =
                enemyUnit.GetAllReachableTiles();
            // Add in the tile the enemy is on to reachableTiles so that GetClosestCoordinateFromList
            // can check if it's the closest tile to the target
            reachableTiles.Add(enemyUnit.Coordinate);
            
            // Can uncomment AND REPLACE THE FOLLOWING LINES if we want enemies to flank to free adjacent squares
            // Vector2Int chosenTargetTile = gridManager.GetClosestCoordinateFromList(targetTiles, enemyUnit.Coordinate);

            Vector2Int chosenTargetTile = gridManager.GetClosestCoordinateFromList(reachableTiles, targetUnit.Coordinate, enemyUnit);

            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy to move to " + chosenTargetTile + " towards " + targetUnit + " at " + targetUnit.Coordinate);
            return chosenTargetTile;
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
                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Targeting closest player " +
                          targetPlayerUnit);
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

        public IUnit Spawn(EnemyUnit unit)
        {
            enemyUnits.Add(unit);
            commandManager.ExecuteCommand(new SpawnedUnitCommand(unit));
            return unit;
        }
    }
}
