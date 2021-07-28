using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abilities;
using Abilities.Commands;
using Cysharp.Threading.Tasks;
using Grid;
using Grid.GridObjects;
using Managers;
using Units.Commands;
using Units.Players;
using Units.Stats;
using UnityEngine;
using Utilities;

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
        
        private GridManager gridManager;
        private PlayerManager playerManager;
        private UnitManager unitManager;

        public override void ManagerStart()
        {
            base.ManagerStart();
            
            gridManager = ManagerLocator.Get<GridManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
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
        
        public IUnit Spawn(EnemyUnit unit)
        {
            enemyUnits.Add(unit);
            commandManager.ExecuteCommand(new SpawnedUnitCommand(unit));
            return unit;
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

        #region ENEMY ACTIONS
        
        public async Task DoUnitAbility(EnemyUnit enemyUnit, Ability ability, Vector2 targetVector)
        {
            Quaternion quaternionTempFix = Quaternion.AngleAxis(45, Vector3.forward);

            commandManager.ExecuteCommand(new AbilityCommand(enemyUnit, quaternionTempFix * targetVector, ability));

            Debug.Log(enemyUnit.Name +
                      " ENEMY-ABL: Enemy is using ability " + ability);

            await commandManager.WaitForCommand<EndUnitCastingCommand>();
            
            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }
        
        public async Task DoUnitAbility(EnemyUnit enemyUnit, Ability ability, IUnit targetUnit) =>
            await DoUnitAbility(enemyUnit, ability, targetUnit.Coordinate - enemyUnit.Coordinate);

        public async Task MoveUnitToTarget(EnemyUnit enemyUnit)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);
            
            var moveCommand = new StartMoveCommand(
                enemyUnit,
                FindClosestPath(enemyUnit, targetPlayerUnit, (int) 
                    enemyUnit.MovementPoints.Value)
            );
            
            commandManager.ExecuteCommand(moveCommand);
            await commandManager.WaitForCommand<EndMoveCommand>();
            
            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }
        
        /// <summary>
        /// Similar to MoveUnitToTarget, but aims for tiles that are <c>distanceFromTarget</c>
        /// away from the target
        /// </summary>
        /// <param name="enemyUnit"></param>
        /// <param name="distanceFromTarget"></param>
        public async Task MoveToTargetRange(EnemyUnit enemyUnit, int distanceFromTarget)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);
            List<Vector2Int> reachableTiles = enemyUnit.GetAllReachableTiles();
            Vector2Int targetTile = new Vector2Int();

            if (reachableTiles.Count <= 0)
                return;

            foreach (var reachableTile in reachableTiles)
            {
                if(distanceFromTarget == ManhattanDistance
                    .GetManhattanDistance(reachableTile, targetPlayerUnit.Coordinate))
                {
                    targetTile = reachableTile;
                    break;
                }
            }

            if (reachableTiles.Contains(targetTile))
            {
                var moveCommand = new StartMoveCommand(
                    enemyUnit,
                    targetTile
                );
                
                Debug.Log(enemyUnit.Name +
                          " ENEMY-TAR: Enemy is moving to "+targetTile+" to maintain a "
                          +distanceFromTarget+" tile distance from "+targetPlayerUnit.Name);
            
                commandManager.ExecuteCommand(moveCommand);
                await commandManager.WaitForCommand<EndMoveCommand>();
            
                while (playerManager.WaitForDeath)
                    await UniTask.Yield();
            }
            else
            {
                // If the enemy can't reach the distanceFromTarget range, they will attempt
                // to move closer to the target
                await MoveUnitToTarget(enemyUnit);
            }
        }
        
        /// <summary>
        /// Finds the tile that is furthest from most players. Only uses reachable tiles.
        /// If there are no reachable tiles, then the enemy will not move.
        /// </summary>
        /// /// <param name="enemyUnit"></param>
        public async Task MoveToDistantTile(EnemyUnit enemyUnit)
        {
            List<Vector2Int> reachableTiles = enemyUnit.GetAllReachableTiles();
            Dictionary<Vector2Int, int> totalTileDistance = new Dictionary<Vector2Int, int>();

            if (reachableTiles.Count <= 0)
                return;
            
            foreach (var reachableTile in reachableTiles)
            {
                int tileDistance = 0;
                
                foreach (var playerUnit in playerManager.PlayerUnits)
                {
                    tileDistance += ManhattanDistance.GetManhattanDistance(
                        reachableTile, playerUnit.Coordinate);
                }
                
                totalTileDistance.Add(reachableTile, tileDistance);
            }

            Vector2Int targetTile = totalTileDistance
                .OrderByDescending(d => d.Value)
                .First().Key;
            
            var moveCommand = new StartMoveCommand(
                enemyUnit,
                targetTile
            );
            
            Debug.Log(enemyUnit.Name +
                      " ENEMY-TAR: Enemy is moving away from players to " + targetTile);
            
            commandManager.ExecuteCommand(moveCommand);
            await commandManager.WaitForCommand<EndMoveCommand>();
            
            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        #endregion

        #region ENEMY FINDING FUNCTIONS

        public GridObject FindAdjacentPlayer(IUnit enemyUnit)
        {
            List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(enemyUnit.Coordinate);

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                    return adjacentGridObject;
            }

            return null;
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

            // Can uncomment if we want enemies to flank to free adjacent squares
            // List<Vector2Int> targetTiles = gridManager.GetAdjacentFreeSquares(targetUnit);

            List<Vector2Int> reachableTiles =
                enemyUnit.GetAllReachableTiles();
            // Add in the tile the enemy is on to reachableTiles so that GetClosestCoordinateFromList
            // can check if it's the closest tile to the target
            reachableTiles.Add(enemyUnit.Coordinate);
            
            // Can uncomment AND REPLACE THE FOLLOWING LINES if we want enemies to flank to free adjacent squares
            // Vector2Int chosenTargetTile = gridManager.GetClosestCoordinateFromList(targetTiles, enemyUnit.Coordinate);

            Vector2Int chosenTargetTile = unitManager.GetClosestCoordinateFromList(reachableTiles, targetUnit.Coordinate, enemyUnit);

            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy to move to " + chosenTargetTile + " towards " + targetUnit + " at " + targetUnit.Coordinate);
            return chosenTargetTile;
        }

        public IUnit GetTargetPlayer(IUnit enemyUnit)
        {
            IUnit targetPlayerUnit;

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
            Dictionary<Vector2Int, int> distanceToAllCells = unitManager.GetDistanceToAllCells(enemyUnit.Coordinate);
            
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

        public List<IUnit> GetLowestHealthPlayers(IReadOnlyList<IUnit> playerUnits)
        {
            List<IUnit> lowestHealthPlayerUnits = new List<IUnit>();
            float lowestHealthValue = Int32.MaxValue;
            
            foreach (var playerUnit in playerUnits)
            {
                if (lowestHealthValue > playerUnit.HealthStat.Value)
                {
                    lowestHealthPlayerUnits.Clear();
                    lowestHealthValue = playerUnit.HealthStat.Value;
                    lowestHealthPlayerUnits.Add(playerUnit);
                }
                else if (lowestHealthValue == playerUnit.HealthStat.Value)
                {
                    lowestHealthPlayerUnits.Add(playerUnit);
                }
            }

            return lowestHealthPlayerUnits;
        }
        
        #endregion
    }
}
