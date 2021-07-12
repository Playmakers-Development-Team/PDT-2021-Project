using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using Managers;
using Units;
using Units.Enemies;
using Units.Players;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
using Random = UnityEngine.Random;
using TileData = Grid.Tiles.TileData;

namespace Grid
{
    public class GridManager : Manager
    {
        public Dictionary<Vector2Int, TileData> tileDatas { get; private set; }
        
        private const int gridLineCastDefaultLimit = 10;

        public Tilemap LevelTilemap { get; private set; }
        public Vector2Int LevelBounds { get; private set; }
        public BoundsInt LevelBoundsInt { get; private set; }

        public void InitialiseGrid(Tilemap levelTilemap, Vector2Int levelBounds)
        {
            tileDatas = new Dictionary<Vector2Int, TileData>();
            
            LevelBounds = levelBounds;
            LevelTilemap = levelTilemap;
            
            LevelBoundsInt = new BoundsInt(
                new Vector3Int(-Mathf.FloorToInt(levelBounds.x / 2.0f), -Mathf.FloorToInt(levelBounds.y / 2.0f), 0),
                new Vector3Int(levelBounds.x - 1, levelBounds.y - 1, 0)
            );

            for (int x = LevelBoundsInt.xMin; x <= LevelBoundsInt.xMax; x++)
            {
                for (int y = LevelBoundsInt.xMin; y <= LevelBoundsInt.yMax; y++)
                {
                    TileBase tile = levelTilemap.GetTile(new Vector3Int(x, y, 0));
                    // This is going to be null, if there is no tile there but that's fine
                    tileDatas.Add(new Vector2Int(x, y), new TileData(tile));

                    // Debug.Log($"Register tiledata at {x}, {y}");
                    //
                    // if (tile != null)
                    // {
                    //     tileDatas.Add(new Vector2Int(x, y), new TileData(tile));
                    // }
                }
            }
        }
        
        #region GETTERS

        public bool IsInBounds(Vector2Int coordinate) =>
            coordinate.x >= LevelBoundsInt.xMin && coordinate.x <= LevelBoundsInt.xMax &&
            coordinate.y >= LevelBoundsInt.yMin && coordinate.y <= LevelBoundsInt.yMax;

        public bool IsInBounds(Vector3 worldPosition, bool clamp = false)
        {
            Vector2Int coordinate = ConvertPositionToCoordinate(worldPosition, clamp);
            return IsInBounds(coordinate);
        }

        public TileData GetTileDataByCoordinate(Vector2Int coordinate)
        {
            if (tileDatas.TryGetValue(coordinate, out TileData tileData))
                return tileData;

            Debug.Log("No tile found at coordinates " + coordinate);
            return null;
        }

        public List<GridObject> GetGridObjectsByCoordinate(Vector2Int coordinate)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData is null)
            {
                Debug.Log("No tile found at coordinates " + coordinate);
                return new List<GridObject>();
            }

            return tileData.GridObjects;
        }

        public Vector2Int GetRandomCoordinates() =>
            new Vector2Int(Random.Range(-(LevelBounds.x / 2), (LevelBounds.x / 2)),
                Random.Range(-(LevelBounds.y / 2), (LevelBounds.y / 2)));

        public Vector2Int GetRandomUnoccupiedCoordinates()
        {
            Vector2Int coordinate = GetRandomCoordinates();

            while (GetGridObjectsByCoordinate(coordinate).Count > 0)
            {
                coordinate = GetRandomCoordinates();
            }

            return coordinate;
        }

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<GridObject> GridLineCast(Vector2Int originCoordinate, Vector2 targetVector,
                                             int limit = gridLineCastDefaultLimit) =>
            GridLineCast(originCoordinate, OrdinalDirectionUtility.From(Vector2.up, targetVector));

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, Vector2 targetVector,
                                       int limit = gridLineCastDefaultLimit) where T : GridObject =>
            GridLineCast<T>(originCoordinate,
                OrdinalDirectionUtility.From(Vector2.up, targetVector), limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<GridObject> GridLineCast(Vector2Int originCoordinate,
                                             OrdinalDirection direction,
                                             int limit = gridLineCastDefaultLimit) =>
            GridLineCast<GridObject>(originCoordinate, direction, limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, OrdinalDirection direction,
                                       int limit = gridLineCastDefaultLimit) where T : GridObject
        {
            Vector2Int increment = direction.ToVector2Int();
            Vector2Int currentCoordinate = originCoordinate;

            for (int i = 0; i < limit; i++)
            {
                currentCoordinate += increment;
                var gridObjects = GetGridObjectsByCoordinate(currentCoordinate);

                if (gridObjects.Count > 0)
                {
                    var foundObjects = gridObjects.OfType<T>().ToList();

                    if (foundObjects.Any())
                        return foundObjects;
                }
            }

            return new List<T>();
        }
        
        public Dictionary<Vector2Int, int> GetDistanceToAllCells(Vector2Int startingCoordinate)
        {
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            string allegiance = "";

            if (tileDatas[startingCoordinate].GridObjects.Count > 0)
            {
                allegiance= tileDatas[startingCoordinate].GridObjects[0].tag;
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
        
        public bool HasPlayerUnitAt(Vector2Int coords) => GetGridObjectsByCoordinate(coords).Any(o => o is PlayerUnit);
        public bool HasEnemyUnitAt(Vector2Int coords) => GetGridObjectsByCoordinate(coords).Any(o => o is EnemyUnit);

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

        [Obsolete("Use Unit.GetAllReachableTiles() instead, this function has been moved to the unit system")]
        public List<Vector2Int> GetAllReachableTiles(Vector2Int startingCoordinate, int range)
        {
            IUnit unit = GetGridObjectsByCoordinate(startingCoordinate)
                .OfType<IUnit>()
                .FirstOrDefault();

            if (unit != null)
            {
                return unit.GetAllReachableTiles();
            }

            Debug.LogWarning($"Obsoleted function can't find unit at coordinate {startingCoordinate}");
            return new List<Vector2Int>();
        }

        #endregion

        #region CONVERSIONS

        public Vector2Int ConvertPositionToCoordinate(Vector2 position, bool clamp = false)
        {
            Vector3Int unbounded = LevelTilemap.layoutGrid.WorldToCell(position);

            if (!clamp)
                return (Vector2Int) unbounded;
            
            return new Vector2Int(
                Mathf.Clamp(unbounded.x, LevelBoundsInt.xMin, LevelBoundsInt.xMax),
                Mathf.Clamp(unbounded.y, LevelBoundsInt.xMin, LevelBoundsInt.xMax)
                );
        }

        public Vector2 ConvertCoordinateToPosition(Vector2Int coordinate) => LevelTilemap.GetCellCenterWorld((Vector3Int) coordinate);

        #endregion

        #region GRID OBJECT FUNCTIONS

        public bool AddGridObject(Vector2Int coordinate, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData != null)
            {
                if (tileData.GridObjects.Count > 0)
                {
                    // Can change this later if we want to allow multiple grid objects on a tile
                    Debug.LogWarning("Failed to add " + gridObject + " at " + coordinate.x + ", " +
                                     coordinate.y + " due to tile being occupied by " +
                                     tileData.GridObjects[0]);
                    return false;
                }

                tileData.AddGridObjects(gridObject);
                Debug.Log(gridObject + " added to tile " + coordinate.x + ", " + coordinate.y);
                return true;
            }

            Debug.LogWarning("Failed to add " + gridObject + " at " + coordinate.x + ", " +
                             coordinate.y + " due to null tileData");

            return false;
        }

        public bool RemoveGridObject(Vector2Int coordinate, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData.GridObjects.Contains(gridObject))
            {
                // Debug.Log("GridObject removed from tile " + coordinate.x + ", " + coordinate.y);
                tileData.RemoveGridObjects(gridObject);
                return true;
            }

            Debug.LogWarning("Failed to remove gridObject at " + coordinate.x + ", " +
                             coordinate.y + ". Tile does not contain gridObject");

            return false;
        }

        public void MoveAllGridObjects(Vector2Int currentCoordinate, Vector2Int newCoordinate)
        {
            List<GridObject> gridObjects = GetGridObjectsByCoordinate(currentCoordinate);

            foreach (var gridObject in gridObjects)
            {
                MoveGridObject(currentCoordinate, newCoordinate, gridObject);
            }
        }
        
        public async UniTask MovementTween(GameObject unit, Vector3 startPos, Vector3 endPos,
                                            float duration)
        {
            float flag = 0f;
            //Debug.Log("Tween unit from " + startPos + " to " + endPos + ".");
            while (flag < duration)
            {
                unit.transform.position = Vector3.Lerp(startPos, endPos, flag / duration);
                flag += Time.deltaTime;
                await UniTask.Yield();
            }
        }
        
        /// <summary>
        /// Moves a unit's GridObject and GameObject directly to a new position.
        /// We need startCoordinate as it cannot be derived from gridObject.Coordinate.
        /// This makes it more reliable (e.g. the function is used after tween movement)
        /// </summary>
        /// <param name="startCoordinate">The coordinate the unit starts on</param>
        /// <param name="newCoordinate">The coordinate to move the unit to.</param>
        /// <param name="gridObject">The gridObject to teleport.</param>
        public void MoveGridObject(Vector2Int startCoordinate, Vector2Int newCoordinate, GridObject gridObject)
        {
            gridObject.gameObject.transform.position = ConvertCoordinateToPosition(newCoordinate);
            
            if (!AddGridObject(newCoordinate, gridObject))
                return;
            
            if (!RemoveGridObject(startCoordinate, gridObject))
                return;
            
            Debug.LogFormat("Moved {0} from {1} to {2}.", gridObject, startCoordinate,
                newCoordinate);
        }

        public List<GridObject> GetAdjacentGridObjects(Vector2Int coordinate)
        {
            GridManager gridManager = ManagerLocator.Get<GridManager>();

            List<GridObject> adjacentGridObjects = new List<GridObject>();
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.up));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.right));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.down));
            adjacentGridObjects.AddRange(
                gridManager.GetGridObjectsByCoordinate(coordinate + Vector2Int.left));

            return adjacentGridObjects;
        }
        
        // TODO: Use for when we want enemies to perform flanking maneuvers
        public List<Vector2Int> GetAdjacentFreeSquares(IUnit targetUnit)
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

        #endregion
    }
}