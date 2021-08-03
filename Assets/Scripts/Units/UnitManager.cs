using System.Collections.Generic;
using Commands;
using Grid;
using Grid.GridObjects;
using Managers;
using Units.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;
using Utilities;

namespace Units
{
    public class UnitManager : Manager
    {
        protected CommandManager commandManager;
        private EnemyManager enemyManager;
        private PlayerManager playerManager;
        private GridManager gridManager;

        /// <summary>
        /// All the units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> AllUnits => GetAllUnits();

        /// <summary>
        /// Initialises the <c>UnitManager</c>.
        /// </summary>
        public override void ManagerStart()
        {
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            gridManager = ManagerLocator.Get<GridManager>();
        }

        /// <summary>
        /// Returns all the units currently in the level.
        /// </summary>
        private List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(enemyManager.Units);
            allUnits.AddRange(playerManager.Units);
            return allUnits;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        public virtual IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitPrefab, gridPosition);
            return unit;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        public IUnit Spawn(string unitName, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitName, gridPosition);
            return unit;
        }
        
        #region Pathfinding
        
        public Dictionary<Vector2Int, int> GetDistanceToAllCells(Vector2Int startingCoordinate)
        {
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            string allegiance = "";

            if (gridManager.GetTileDataByCoordinate(startingCoordinate).GridObjects.Count > 0)
            {
                allegiance = gridManager.GetTileDataByCoordinate(startingCoordinate).GridObjects[0].tag;
            }

            // Add the starting coordinate to the queue
            coordinateQueue.Enqueue(startingCoordinate);
            int distance = 0;
            visited.Add(startingCoordinate, distance);
            
            // Loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];

                // Add neighbours of node to queue
                Pathfinding.VisitDistanceToAllNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                Pathfinding.VisitDistanceToAllNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                Pathfinding.VisitDistanceToAllNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                Pathfinding.VisitDistanceToAllNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                
                coordinateQueue.Dequeue();
            }

            return visited;
        }

        /// <summary>
        /// Returns the coordinate that is closest to the destination
        /// from a list of coordinates. Will find the closest tile even
        /// if targetCoordinate is not in range
        /// </summary>
        public Vector2Int GetClosestCoordinateFromList(List<Vector2Int> reachableCoordinates,
                                                       Vector2Int targetCoordinate, IUnit iunit)
        {
            // PLACEHOLDER INITIALISATION
            Vector2Int closestTile = reachableCoordinates[0];
            int shortestDistance = int.MaxValue;
            
            foreach (var startingCoordinate in reachableCoordinates)
            {
                List<Vector2Int> pathToTargetTile = Pathfinding.GetCellPath(targetCoordinate, startingCoordinate, iunit);
                
                //Debug.Log("Tile Coordinate: "+startingCoordinate+". TargetCoordinate(enemy): "+targetCoordinate+" Count: "+pathToTargetTile.Count);
                
                if (pathToTargetTile.Count != 0 && pathToTargetTile.Count < shortestDistance)
                {
                    shortestDistance = pathToTargetTile.Count;
                    closestTile = startingCoordinate;
                }
            }
            
            if (shortestDistance != int.MaxValue)
                return closestTile;
            
            Debug.LogWarning($"Could not find tile to move to, returning {reachableCoordinates[0]}");
            return reachableCoordinates[0];
            
            //Debug.Log("Chosen Tile Coordinate: "+closestTile);
        }

        #endregion
    }

    public abstract class UnitManager<T> : UnitManager where T : UnitData
    {
        private readonly List<IUnit> units = new List<IUnit>();

        public IReadOnlyList<IUnit> Units => units.AsReadOnly();

        public void ClearUnits() => units.Clear();
        
        public GridObject FindAdjacentPlayer(IUnit unit)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();
            List<GridObject> adjacentGridObjects = gridManager.GetAdjacentGridObjects(unit.Coordinate);

            foreach (var adjacentGridObject in adjacentGridObjects)
            {
                if (adjacentGridObject.CompareTag("PlayerUnit"))
                    return adjacentGridObject;
            }

            return null;
        }

        public IUnit Spawn(IUnit unit)
        {
            units.Add(unit);
            commandManager.ExecuteCommand(new SpawnedUnitCommand(unit));
            return unit;
        }
        
        /// <summary>
        /// Removes a target <c>IUnit</c> from the <c>units</c> list.
        /// </summary>
        public void RemoveUnit(IUnit targetUnit) => units.Remove(targetUnit);
        
        /// <summary>
        /// Adds a unit to the <c>units</c> list.
        /// </summary>
        public void AddUnit(IUnit targetUnit) => units.Add(targetUnit);
        
        /// <summary>
        /// Spawns a unit and adds it to the <c>units</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c> that was added.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
            units.Add(newUnit);
            commandManager.ExecuteCommand(new SpawnedUnitCommand(newUnit));
            return newUnit;
        }
    }
}