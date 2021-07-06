using System.Collections.Generic;
using System.Linq;
using GridObjects;
using Units;
using Units.Commands;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : Manager
    {
        public Dictionary<Vector2Int, TileData> tileDatas { get; private set; }
        
        private const int GridLineCastDefaultLimit = 10;

        public Tilemap LevelTilemap { get; private set; }
        public Vector2Int LevelBounds { get; private set; }
        public Vector2 GridOffset { get; private set; }

        public void InitialiseGrid(Tilemap levelTilemap, Vector2Int levelBounds, Vector2 gridOffset)
        {
            tileDatas = new Dictionary<Vector2Int, TileData>();
            
            LevelBounds = levelBounds;
            LevelTilemap = levelTilemap;
            GridOffset = gridOffset;

            for (int x = -levelBounds.x / 2 - 1; x <= levelBounds.x / 2; x++)
            {
                for (int y = -levelBounds.y / 2 - 1; y <= levelBounds.y / 2; y++)
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

        public Vector2Int GetRandomCoordinates()
        {
            return new Vector2Int(Random.Range(-(LevelBounds.x / 2), (LevelBounds.x / 2)),
                Random.Range(-(LevelBounds.y / 2), (LevelBounds.y / 2)));
        }

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
                                             int limit = GridLineCastDefaultLimit) =>
            GridLineCast(originCoordinate, OrdinalDirectionUtility.From(Vector2.up, targetVector));

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, Vector2 targetVector,
                                       int limit = GridLineCastDefaultLimit) where T : GridObject =>
            GridLineCast<T>(originCoordinate,
                OrdinalDirectionUtility.From(Vector2.up, targetVector), limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<GridObject> GridLineCast(Vector2Int originCoordinate,
                                             OrdinalDirection direction,
                                             int limit = GridLineCastDefaultLimit) =>
            GridLineCast<GridObject>(originCoordinate, direction, limit);

        /// <summary>
        /// Similar to ray casting but done on the grid space.
        /// </summary>
        /// <returns>All the GridObjects found at the coordinate of the first found target.</returns>
        public List<T> GridLineCast<T>(Vector2Int originCoordinate, OrdinalDirection direction,
                                       int limit = GridLineCastDefaultLimit) where T : GridObject
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
                VisitDistanceToAllNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                VisitDistanceToAllNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance, coordinateQueue, allegiance);
                
                coordinateQueue.Dequeue();
            }

            return visited;
        }

        public void VisitNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance,
                               Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag))
                {
                    // Check node has not already been visited
                    if (!visited.ContainsKey(node))
                    {
                        // Add node to queue and store the distance taken to arrive at it
                        visited.Add(node, distance + 1);
                        coordinateQueue.Enqueue(node);
                    }
                }
            }
        }
        
        private void VisitDistanceToAllNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance,
                               Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag))
                {
                    // Check node has not already been visited
                    if (!visited.ContainsKey(node))
                    {
                        // Add node to queue and store the distance taken to arrive at it
                        visited.Add(node, distance + 1);
                        coordinateQueue.Enqueue(node);
                    }
                }else if(tileDatas[node].GridObjects[0].tag.Equals("PlayerUnit"))
                {
                    if (!visited.ContainsKey(node))
                    {
                        visited.Add(node, distance + 1);
                    }
                }
            }
            
        }

        private void VisitPathNode(Vector2Int node, Dictionary<Vector2Int, Vector2Int> visited,
                               Queue<Vector2Int> coordinateQueue, string allegiance)
        {
            // Check grid node exists
            if (tileDatas.ContainsKey(node))
            {
                // Check node is empty or matches allegiance
                if (tileDatas[node].GridObjects.Count == 0 ||
                    allegiance.Equals(tileDatas[node].GridObjects[0].tag))
                {
                    // Check node has not already been visited
                    if (!visited.ContainsKey(node) && !visited.ContainsValue(node))
                    {
                        // Add node to queue and store the previous node
                        visited.Add(node, coordinateQueue.Peek());
                        coordinateQueue.Enqueue(node);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of the path from one node to another
        /// Assumes target is reachable.
        /// </summary>
        public List<Vector2Int> GetCellPath(Vector2Int startingCoordinate,
                                             Vector2Int targetCoordinate)
        {
            var visited = new Dictionary<Vector2Int, Vector2Int>();
            var coordinateQueue = new Queue<Vector2Int>();
            string allegiance = string.Empty;

            if (GetGridObjectsByCoordinate(startingCoordinate).Count > 0)
            {
                allegiance = GetTileDataByCoordinate(startingCoordinate).GridObjects[0].gameObject.tag;
            }

            coordinateQueue.Enqueue(startingCoordinate);
            while (coordinateQueue.Count > 0)
            {
                var currentNode = coordinateQueue.Peek();

                VisitPathNode(currentNode + CardinalDirection.North.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitPathNode(currentNode + CardinalDirection.East.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitPathNode(currentNode + CardinalDirection.South.ToVector2Int(), visited,
                    coordinateQueue, allegiance);
                VisitPathNode(currentNode + CardinalDirection.West.ToVector2Int(), visited,
                    coordinateQueue, allegiance);

                if (visited.ContainsKey(targetCoordinate))
                    coordinateQueue.Clear();
                else
                    coordinateQueue.Dequeue();
            }

            foreach (KeyValuePair<Vector2Int, Vector2Int> node in visited)
            {
                //Debug.Log($"NodeStuff {node}");
            }

            var path = new List<Vector2Int>();
            var currentNode2 = targetCoordinate;
            int count = 0;
            while (count < 20)
            {
                path.Add(currentNode2);
                if (visited.ContainsKey(currentNode2))
                    currentNode2 = visited[currentNode2];
                else
                    break;

                count++;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Returns the coordinate that is closest to the destination
        /// from a list of coordinates
        /// </summary>
        public Vector2Int GetClosestCoordinateFromList(List<Vector2Int> startingCoordinates,
                                            Vector2Int targetCoordinate)
        {
            // PLACEHOLDER INITIALISATION
            Vector2Int closestTile = startingCoordinates[0];
            int shortestDistance = int.MaxValue;
            
            foreach (var startingCoordinate in startingCoordinates)
            {
                List<Vector2Int> pathToTargetTile = GetCellPath(targetCoordinate, startingCoordinate);
                
                //Debug.Log("Tile Coordinate: "+startingCoordinate+". TargetCoordinate(enemy): "+targetCoordinate+" Count: "+pathToTargetTile.Count);
                
                if (pathToTargetTile.Count < shortestDistance)
                {
                    shortestDistance = pathToTargetTile.Count;
                    closestTile = startingCoordinate;
                }
            }
            //Debug.Log("Chosen Tile Coordinate: "+closestTile);
            return closestTile;
        }

        #endregion

        #region CONVERSIONS

        public Vector2Int ConvertPositionToCoordinate(Vector2 position)
        {
            // Debug.Log("WorldSpace: " + worldSpace + " | GridSpace: " + 
            //           (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace));
            return (Vector2Int) LevelTilemap.layoutGrid.WorldToCell(position);
        }

        public Vector2 ConvertCoordinateToPosition(Vector2Int coordinate)
        {
            // Debug.Log("GridSpace: " + gridSpace + " | WorldSpace: " + 
            //           levelTilemap.layoutGrid.CellToWorld((Vector3Int) gridSpace));
            return (Vector2) LevelTilemap.layoutGrid.CellToWorld((Vector3Int) coordinate) +
                   GridOffset;
        }

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
                MoveGridObject(newCoordinate, gridObject);
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
        /// </summary>
        /// <param name="newCoordinate">The coordinate to move the unit to.</param>
        /// <param name="gridObject">The gridObject to teleport.</param>
        public void MoveGridObject(Vector2Int newCoordinate, GridObject gridObject)
        {
            Vector2Int startCoordinate = gridObject.Coordinate;
            
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