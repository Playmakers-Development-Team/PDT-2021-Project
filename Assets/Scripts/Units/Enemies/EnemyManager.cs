using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abilities;
using Abilities.Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Units.Commands;
using Units.Players;
using UnityEngine;
using Utilities;

namespace Units.Enemies
{
    public class EnemyManager : UnitManager<EnemyUnitData>
    {
        private PlayerManager playerManager;
        private UnitManager unitManager;

        public override void ManagerStart()
        {
            base.ManagerStart();
            
            playerManager = ManagerLocator.Get<PlayerManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
        }

        public async Task Spawner(EnemySpawnerUnit spawnUnit)
        {
            // Get spawner stats
            int damage = spawnUnit.HealthStat.BaseValue - spawnUnit.HealthStat.Value;
            int curSpeed = spawnUnit.SpeedStat.Value;
            Vector2Int unitPosition = spawnUnit.UnitPosition;

            // Kill spawner
            spawnUnit.TakeDamage(spawnUnit.HealthStat.Value + spawnUnit.DefenceStat.Value + 20);
            await commandManager.WaitForCommand<KilledUnitCommand>();

            // Spawn unit
            GameObject spawnPrefab = spawnUnit.SpawnPrefab;
            spawnPrefab.GetComponent<EnemyUnit>().HealthStat.BaseValue = 5;
            EnemyUnit enemyUnit = (EnemyUnit)Spawn(spawnPrefab, unitPosition);
            await commandManager.WaitForCommand<SpawnedUnitCommand>(); //IMPORTANT

            // Apply spawner stats
            enemyUnit.SetSpeed(curSpeed);
            enemyUnit.TakeDamage(damage);

            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(spawnUnit));
        }
        
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
            
            if (enemyUnit.GetAllReachableTiles().Count <= 0 || targetPlayerUnit is null)
                return;
            
            var moveCommand = new StartMoveCommand(
                enemyUnit,
                FindClosestPath(enemyUnit, targetPlayerUnit, (int) 
                    enemyUnit.MovementPoints.Value)
            );
            
            if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                return;
            
            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy to move to " +
                      moveCommand.TargetCoords + " towards " + targetPlayerUnit + 
                      " at " + targetPlayerUnit.Coordinate);
            
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

            if (reachableTiles.Count <= 0 || targetPlayerUnit is null)
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
                
                if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                    return;
                
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
                
                foreach (var playerUnit in playerManager.Units)
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
            
            if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                return;
            
            Debug.Log(enemyUnit.Name +
                      " ENEMY-TAR: Enemy is moving away from players to " + targetTile);
            
            commandManager.ExecuteCommand(moveCommand);
            await commandManager.WaitForCommand<EndMoveCommand>();
            
            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        #endregion

        #region ENEMY FINDING FUNCTIONS

        private Vector2Int FindClosestPath(EnemyUnit enemyUnit, IUnit targetUnit, int movementPoints)
        {
            if (movementPoints <= 0)
            {
                Debug.Log(enemyUnit.Name +
                          " ENEMY-TAR: Enemy is stationary as it has no movement points");
                return Vector2Int.zero;
            }

            // Can uncomment if we want enemies to flank to free adjacent squares
            // List<Vector2Int> targetTiles = gridManager.GetAdjacentFreeSquares(targetUnit);

            List<Vector2Int> reachableTiles = enemyUnit.GetAllReachableTiles();
            // Add in the tile the enemy is on to reachableTiles so that GetClosestCoordinateFromList
            // can check if it's the closest tile to the target
            reachableTiles.Add(enemyUnit.Coordinate);
            
            // Can uncomment AND REPLACE THE FOLLOWING LINES if we want enemies to flank to free adjacent squares
            // Vector2Int chosenTargetTile = gridManager.GetClosestCoordinateFromList(targetTiles, enemyUnit.Coordinate);

            Vector2Int chosenTargetTile = unitManager.GetClosestCoordinateFromList(reachableTiles, targetUnit.Coordinate, enemyUnit);
            return chosenTargetTile;
        }

        public IUnit GetTargetPlayer(IUnit enemyUnit)
        {
            IUnit targetPlayerUnit;

            List<IUnit> closestPlayers = GetClosestPlayers(enemyUnit, playerManager.Units);
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
                Debug.Log(enemyUnit.Name + " used GetTargetPlayer() called but no players are reachable");
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
                if (distanceToAllCells.ContainsKey(playerUnit.Coordinate))
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
