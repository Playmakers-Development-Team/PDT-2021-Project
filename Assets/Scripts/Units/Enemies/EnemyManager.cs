using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abilities;
using Abilities.Commands;
using Abilities.Shapes;
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

        #region ENEMY ACTIONS

        public async Task DoUnitAbility(EnemyUnit enemyUnit, Ability ability,
                                        ShapeDirection shapeDirection)
        {
            commandManager.ExecuteCommand(new AbilityCommand(enemyUnit, shapeDirection, ability));

            Debug.Log(enemyUnit.Name + " ENEMY-ABL: Enemy is using ability " + ability);

            await commandManager.WaitForCommand<EndUnitCastingCommand>();

            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        public async Task DoUnitAbility(EnemyUnit enemyUnit, Ability ability, IUnit targetUnit) =>
            await DoUnitAbility(enemyUnit, ability, ShapeDirection.Towards(enemyUnit, targetUnit));

        public async Task MoveUnitToTarget(EnemyUnit enemyUnit) =>
            await MoveUnitToTarget(enemyUnit, enemyUnit.GetAllReachableTiles());

        public async Task MoveUnitToTarget(EnemyUnit enemyUnit,
                                           IEnumerable<Vector2Int> reachableTiles)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);

            if (!reachableTiles.Any() || targetPlayerUnit is null)
                return;

            var moveCommand = new StartMoveCommand(enemyUnit,
                FindClosestPath(enemyUnit, targetPlayerUnit, (int) enemyUnit.MovementPoints.Value,
                    reachableTiles));

            if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                return;

            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy to move to " + moveCommand.TargetCoords +
                      " towards " + targetPlayerUnit + " at " + targetPlayerUnit.Coordinate);

            commandManager.ExecuteCommand(moveCommand);
            await commandManager.WaitForCommand<EndMoveCommand>();

            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        /// <summary>
        /// Similar to MoveUnitToTarget, but aims for tiles that are <c>distanceFromTarget</c>
        /// away from the target.
        /// </summary>
        public async UniTask MoveToTargetRange(EnemyUnit enemyUnit, Ability abilityToHit,
                                               int distanceFromTarget)
        {
            List<Vector2Int> reachableTiles = enemyUnit.GetAllReachableTiles();
            await MoveToTargetRange(enemyUnit, abilityToHit, distanceFromTarget, reachableTiles);
        }

        /// <summary>
        /// Similar to MoveUnitToTarget, but aims for tiles that are <c>distanceFromTarget</c>
        /// away from the target.
        /// Remember to include the starting coordinate if not moving should be an option. In most cases it should be an option.
        /// </summary>
        public async UniTask MoveToTargetRange(EnemyUnit enemyUnit, Ability abilityToHit,
                                               int distanceFromTarget,
                                               ICollection<Vector2Int> reachableTiles)
        {
            IUnit targetPlayerUnit = GetTargetPlayer(enemyUnit);

            if (reachableTiles.Count <= 0 || targetPlayerUnit is null || distanceFromTarget <= 1)
                return;

            // Not moving is sometimes the best option.
            reachableTiles.Add(enemyUnit.Coordinate);

            // We can only go to tile that is more than the distanceFromTarget, but we also the closest
            var safestClosestTiles = reachableTiles.
                Where(coor =>
                    ManhattanDistance.GetManhattanDistance(coor, targetPlayerUnit.Coordinate) >=
                    distanceFromTarget).
                OrderBy(coor =>
                    ManhattanDistance.GetManhattanDistance(coor, targetPlayerUnit.Coordinate));

            // Get tiles that if we were to move to, we can hit the target player
            var tilesThatCanTargetPlayer = safestClosestTiles.Where(coor =>
                GetTargetsInRange(coor, abilityToHit).Contains(targetPlayerUnit));

            if (tilesThatCanTargetPlayer.Any())
            {
                var targetTile = tilesThatCanTargetPlayer.First();

                var moveCommand = new StartMoveCommand(enemyUnit, targetTile);

                if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                    return;

                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy is moving to " + targetTile +
                          " to maintain a " + distanceFromTarget + " tile distance from " +
                          targetPlayerUnit.Name);

                commandManager.ExecuteCommand(moveCommand);
                await commandManager.WaitForCommand<EndMoveCommand>();

                while (playerManager.WaitForDeath)
                    await UniTask.Yield();
            }
            else
            {
                // Can the player reach this enemy?
                // We want to get the diamond shaped movement range from the player as an approximation for whether
                // the enemy should try to stay away or get closer to the player
                var hittableTilesFromPlayer = targetPlayerUnit.GetMaxReachableOccupiedTiles();

                // We move closer if we are far away from the target away
                if (
                    !hittableTilesFromPlayer.Contains(enemyUnit.
                        Coordinate)) // await MoveToTargetRange(enemyUnit, abilityToHit, distanceFromTarget - 1, reachableTiles);
                    await MoveUnitToTarget(enemyUnit, reachableTiles);
                else
                    await MoveToDistantTile(enemyUnit, reachableTiles);
            }
        }

        /// <summary>
        /// Finds the tile that is furthest from most players. Only uses reachable tiles.
        /// If there are no reachable tiles, then the enemy will not move.
        /// </summary>
        public async UniTask MoveToDistantTile(EnemyUnit enemyUnit) =>
            await MoveToDistantTile(enemyUnit, enemyUnit.GetAllReachableTiles());

        /// <summary>
        /// Finds the tile that is furthest from most players given the set of reachable tiles
        /// If there are no reachable tiles, then the enemy will not move.
        /// </summary>
        public async UniTask MoveToDistantTile(EnemyUnit enemyUnit,
                                               IEnumerable<Vector2Int> reachableTiles)
        {
            Dictionary<Vector2Int, int> totalTileDistance = new Dictionary<Vector2Int, int>();

            if (!reachableTiles.Any())
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

            Vector2Int targetTile = totalTileDistance.OrderByDescending(d => d.Value).First().Key;

            var moveCommand = new StartMoveCommand(enemyUnit, targetTile);

            if (moveCommand.TargetCoords == enemyUnit.Coordinate)
                return;

            Debug.Log(enemyUnit.Name + " ENEMY-TAR: Enemy is moving away from players to " +
                      targetTile);

            commandManager.ExecuteCommand(moveCommand);
            await commandManager.WaitForCommand<EndMoveCommand>();

            while (playerManager.WaitForDeath)
                await UniTask.Yield();
        }

        #endregion

        #region ENEMY FINDING FUNCTIONS

        private Vector2Int
            FindClosestPath(EnemyUnit enemyUnit, IUnit targetUnit, int movementPoints) =>
            FindClosestPath(enemyUnit, targetUnit, movementPoints,
                enemyUnit.GetAllReachableTiles());

        private Vector2Int FindClosestPath(EnemyUnit enemyUnit, IUnit targetUnit,
                                           int movementPoints,
                                           IEnumerable<Vector2Int> reachableTiles)
        {
            if (movementPoints <= 0)
            {
                Debug.Log(enemyUnit.Name +
                          " ENEMY-TAR: Enemy is stationary as it has no movement points");
                return Vector2Int.zero;
            }

            // Can uncomment if we want enemies to flank to free adjacent squares
            // List<Vector2Int> targetTiles = gridManager.GetAdjacentFreeSquares(targetUnit);

            // List<Vector2Int> reachableTilesList = enemyUnit.GetAllReachableTiles();
            List<Vector2Int> reachableTilesList = reachableTiles.ToList();

            // Add in the tile the enemy is on to reachableTiles so that GetClosestCoordinateFromList
            // can check if it's the closest tile to the target
            reachableTilesList.Add(enemyUnit.Coordinate);

            // Can uncomment AND REPLACE THE FOLLOWING LINES if we want enemies to flank to free adjacent squares
            // Vector2Int chosenTargetTile = gridManager.GetClosestCoordinateFromList(targetTiles, enemyUnit.Coordinate);

            Vector2Int chosenTargetTile =
                unitManager.GetClosestCoordinateFromList(reachableTilesList, targetUnit.Coordinate,
                    enemyUnit);
            return chosenTargetTile;
        }

        /// <summary>
        /// Returns all players within <c>shootingRange</c> tiles of the enemy.
        /// Assumes that all obstacles cannot be shot through for now
        /// </summary>
        public List<IUnit> GetTargetsInRange(Vector2Int coordinate, Ability ability) =>
            ability.Shape.GetTargetsInAllDirections(coordinate).
                OfType<PlayerUnit>().
                OfType<IUnit>().
                ToList();

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

                Debug.Log(enemyUnit.Name + " ENEMY-TAR: Targeting lower HP player " +
                          targetPlayerUnit + "(Multiple closest players found)");
            }
            else
            {
                Debug.Log(enemyUnit.Name +
                          " used GetTargetPlayer() called but no players are reachable");
                return null;
            }

            return targetPlayerUnit;
        }

        public List<IUnit> GetClosestPlayers(IUnit enemyUnit, IReadOnlyList<IUnit> playerUnits)
        {
            Dictionary<Vector2Int, int> distanceToAllCells =
                unitManager.GetDistanceToAllCells(enemyUnit.Coordinate);

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
