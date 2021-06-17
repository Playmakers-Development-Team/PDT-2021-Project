using System;
using System.Collections.Generic;
using System.Linq;
using GridObjects;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
using Random = UnityEngine.Random;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : Manager
    {
        private Dictionary<Vector2Int, TileData> tileDatas = new Dictionary<Vector2Int, TileData>();

        private const int GridLineCastDefaultLimit = 10;

        public Tilemap LevelTilemap { get; private set; }
        public Vector2Int LevelBounds { get; private set; }
        public Vector2 GridOffset { get; private set; }

        public void InitialiseGrid(Tilemap levelTilemap, Vector2Int levelBounds, Vector2 gridOffset)
        {
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
            if(tileDatas.TryGetValue(coordinate, out TileData tileData))
            {
                return tileData;
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + coordinate);
            return null;
        }
        
        public List<GridObject> GetGridObjectsByCoordinate(Vector2Int coordinate)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData is null)
            {
                Debug.LogError("ERROR: No tileData was found for the provided coordinates " + coordinate);
                return null;
            }
            
            return tileData.GridObjects;
        }

        public Vector2Int GetRandomCoordinates()
        {
            return new Vector2Int(
                Random.Range(-(LevelBounds.x / 2), (LevelBounds.x / 2)), 
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
            GridLineCast<T>(originCoordinate, OrdinalDirectionUtility.From(Vector2.up, targetVector), limit);

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
        
        /// <summary>
        /// Returns a list of all coordinates that are reachable from a given starting position
        /// within the given range.
        /// </summary>
        /// <param name="startingCoordinate">The coordinate to begin the search from.</param>
        /// <param name="range">The range from the starting tile using manhattan distance.</param>
        /// <returns>A list of the coordinates of reachable tiles.</returns>
        public List<Vector2Int> AllReachableTiles(Vector2Int startingCoordinate, int range)
        {
            List<Vector2Int> reachable = new List<Vector2Int>();
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            
            // Add the starting coordinate to the queue
            coordinateQueue.Enqueue(startingCoordinate);
            int distance = 0;
            visited.Add(startingCoordinate, distance);
            
            // Loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];
                
                if (distance > range) { break;}
                
                // Add neighbours of node to queue
                VisitNode(currentNode + CardinalDirection.North.ToVector2Int(), visited, distance, coordinateQueue);
                VisitNode(currentNode + CardinalDirection.East.ToVector2Int(), visited, distance, coordinateQueue);
                VisitNode(currentNode + CardinalDirection.South.ToVector2Int(), visited, distance, coordinateQueue);
                VisitNode(currentNode + CardinalDirection.West.ToVector2Int(), visited, distance, coordinateQueue);
                
                reachable.Add(currentNode);
                coordinateQueue.Dequeue();
            }
            return reachable;
        }
        private void VisitNode(Vector2Int node, Dictionary<Vector2Int, int> visited, int distance, Queue<Vector2Int> coordinateQueue)
        {
            // If grid node exists add to queue and mark distance taken to arrive at it
            if (tileDatas.ContainsKey(node) && tileDatas[node].GridObjects.Count == 0)
            {
                if (!visited.ContainsKey(node))
                {
                    visited.Add(node, distance + 1);
                    coordinateQueue.Enqueue(node);
                }
            }
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
            return (Vector2) LevelTilemap.layoutGrid.CellToWorld((Vector3Int) coordinate) + GridOffset;
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
                    Debug.LogWarning("Failed to add " + gridObject +
                                     " at " + coordinate.x + ", " + coordinate.y +
                                     " due to tile being occupied by " + tileData.GridObjects[0]);
                    return false;
                }

                Debug.Log(gridObject + " added to tile " + coordinate.x + ", " + coordinate.y);
                tileData.AddGridObjects(gridObject);
                return true;
            }
            
            Debug.LogWarning("Failed to add " + gridObject + 
                             " at " + coordinate.x + ", " + coordinate.y +
                             " due to null tileData");
            
            return false;
        }
        
        public bool RemoveGridObject(Vector2Int coordinate, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData.GridObjects.Contains(gridObject))
            {
                Debug.Log("GridObject removed from tile " + coordinate.x + ", " + coordinate.y);
                tileData.RemoveGridObjects(gridObject);
                return true;
            }
            
            Debug.LogWarning("Failed to remove gridObject at " + coordinate.x + ", " + coordinate.y + 
                      ". Tile does not contain gridObject");
        
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
        

        // TODO: CurrentCoordinate should not be necessary
        public void MoveUnit(Vector2Int currentCoordinate, Vector2Int newCoordinate, IUnit unit)
        {
            TileData tileData = GetTileDataByCoordinate(newCoordinate);
            
            // TODO: Expose this variable
            int moveRange = (int)unit.MovementActionPoints.Value;
            
            // Check if tile is unoccupied
            if (tileData.GridObjects.Count != 0)
            {
                // TODO: Provide feedback to the player
                Debug.Log("Target tile is occupied.");
                return;
            }
            
            // Check if tile is in range
            if (!AllReachableTiles(currentCoordinate, moveRange).Contains(newCoordinate))
            {
                // TODO: Provide feedback to the player
                Debug.Log("Target tile out of range.");
                return;
            }
            
            TeleportUnit(currentCoordinate, newCoordinate, unit);
        }

        // TODO: CurrentCoordinate should not be necessary
        /// <summary>
        /// Moves a unit's GridObject and GameObject directly to a new position.
        /// </summary>
        /// <param name="currentCoordinate">The unit's current coordinate.</param>
        /// <param name="newCoordinate">The coordinate to move the unit to.</param>
        /// <param name="unit">The unit to teleport.</param>
        private void TeleportUnit(Vector2Int currentCoordinate, Vector2Int newCoordinate, IUnit unit)
        {
            var gridObject = (GridObject) unit;
            
            gridObject.gameObject.transform.position = ConvertCoordinateToPosition(newCoordinate);
            
            MoveGridObject(currentCoordinate, newCoordinate, gridObject);
        }

        // TODO: CurrentCoordinate should not be necessary
        public void MoveGridObject(Vector2Int currentCoordinate, Vector2Int newCoordinate, GridObject gridObject)
        {
            if (AddGridObject(newCoordinate, gridObject))
            {
                RemoveGridObject(currentCoordinate, gridObject);
            }
        }
        
        #endregion
    }
}