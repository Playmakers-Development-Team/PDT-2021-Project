using System.Collections.Generic;
using Commands;
using Grid;
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
        public IUnit SelectedUnit { get; private set; }
        
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
            allUnits.AddRange(enemyManager.EnemyUnits);
            allUnits.AddRange(playerManager.PlayerUnits);
            return allUnits;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitPrefab, gridPosition);
            return unit;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(string unitName, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitName, gridPosition);
            return unit;
        }
        
        /// <summary>
        /// Sets a unit as selected.
        /// </summary>
        /// <param name="unit"></param>
        public void SelectUnit(IUnit unit)
        {
            if (unit is null)
            {
                Debug.LogWarning($"{nameof(UnitManager)}.{nameof(SelectUnit)} should not be " + 
                    $"passed a null value. Use {nameof(UnitManager)}.{nameof(DeselectUnit)} instead.");
                
                DeselectUnit();
                
                return;
            }

            if (SelectedUnit == unit)
                return;

            SelectedUnit = unit;
            commandManager.ExecuteCommand(new UnitSelectedCommand(SelectedUnit));
        }

        /// <summary>
        /// Deselects the currently selected unit.
        /// </summary>
        public void DeselectUnit()
        {
            if (SelectedUnit is null)
                return;
            
            Debug.Log(SelectedUnit + " deselected.");
            commandManager.ExecuteCommand(new UnitDeselectedCommand(SelectedUnit));
            SelectedUnit = null;
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
            {
                return closestTile;
            }
            else
            {
                Debug.LogWarning($"Could not find tile to move to, returning {reachableCoordinates[0]}");
                return reachableCoordinates[0];
            }
            //Debug.Log("Chosen Tile Coordinate: "+closestTile);
        }

        #endregion
    }
}