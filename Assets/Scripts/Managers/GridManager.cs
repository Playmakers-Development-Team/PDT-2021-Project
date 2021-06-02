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

        public Tilemap levelTilemap { get; set; }

        public void InitialiseTileDatas()
        {
            BoundsInt bounds = levelTilemap.cellBounds;

            for (int x = -bounds.size.x/2 - 1; x <= bounds.size.x/2; x++)
            {
                for (int y = -bounds.size.y/2 - 1; y <= bounds.size.y/2; y++)
                {
                    TileBase tile = levelTilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile != null)
                    {
                        tileDatas.Add(new Vector2Int(x, y), new TileData(tile));
                    }
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
            BoundsInt bounds = levelTilemap.cellBounds;
            
            return new Vector2Int(
                Random.Range(bounds.xMin, bounds.xMax), 
                Random.Range(bounds.yMin, bounds.yMax));
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
        
        public List<Vector2Int> allReachableTiles(Vector2Int startingPosition, int range, Dictionary<Vector2Int, TileData> grid)
        {
            //Returns a list of all coordinates reachablefrom a startting position within a set range
            
            List<Vector2Int> reachable = new List<Vector2Int>();
            Dictionary<Vector2Int, int> visited = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> coordinateQueue = new Queue<Vector2Int>();
            
            //add starting node
            coordinateQueue.Enqueue(startingPosition);
            int distance = 0;
            visited.Add(startingPosition, distance);
            
            //loop until all nodes are processed
            while (coordinateQueue.Count > 0)
            {
                Vector2Int currentNode = coordinateQueue.Peek();
                distance = visited[currentNode];
                //Debug.Log("On this node now"+current);
                if (distance > range) { break;}
                
                //add neighbours of node to queue
                visitNode(grid, currentNode + CardinalDirection.North.ToVector2Int(), visited, distance, coordinateQueue);
                visitNode(grid, currentNode + CardinalDirection.East.ToVector2Int(), visited, distance, coordinateQueue);
                visitNode(grid, currentNode + CardinalDirection.South.ToVector2Int(), visited, distance, coordinateQueue);
                visitNode(grid, currentNode + CardinalDirection.West.ToVector2Int(), visited, distance, coordinateQueue);
                
                reachable.Add(currentNode);
                coordinateQueue.Dequeue();
            }

            return reachable;
        }
        private void visitNode(Dictionary<Vector2Int, TileData> grid, Vector2Int node, Dictionary<Vector2Int, int> visited, int distance, Queue<Vector2Int> coordinateQueue)
        {
            //Iff grid node exists add to queue and mark distance taken to arrive at it
            if ((grid.ContainsKey(node)) && grid[node].GridObjects.Count == 0)
            {
                if (!(visited.ContainsKey(node)))
                {
                    //Debug.Log("Adding " + node + " with distance" + distance + 1);
                    visited.Add(node, distance + 1);
                    coordinateQueue.Enqueue(node);
                }
            }
        }
        
        #endregion

        #region CONVERSIONS

        [Obsolete("Use ConvertPositionToCoordinate instead from now on")]
        public Vector2Int ConvertWorldSpaceToGridSpace(Vector2 worldSpace) =>
            ConvertPositionToCoordinate(worldSpace);

        [Obsolete("Use ConvertCoordinateToPosition instead from now on")]
        public Vector2 ConvertGridSpaceToWorldSpace(Vector2Int gridSpace) =>
            ConvertCoordinateToPosition(gridSpace);
        
        public Vector2Int ConvertPositionToCoordinate(Vector2 position)
        {
            // Debug.Log("WorldSpace: " + worldSpace + " | GridSpace: " + 
            //           (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace));
            return (Vector2Int) levelTilemap.layoutGrid.WorldToCell(position);
        }
        
        public Vector2 ConvertCoordinateToPosition(Vector2Int coordinate)
        {
            // Debug.Log("GridSpace: " + gridSpace + " | WorldSpace: " + 
            //           levelTilemap.layoutGrid.CellToWorld((Vector3Int) gridSpace));
            return levelTilemap.layoutGrid.CellToWorld((Vector3Int) coordinate);
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
        
        public void MoveObjectsFromTile(Vector2Int currentPosition, Vector2Int newPosition)
        {
            List<GridObject> gridObjects = GetGridObjectsByCoordinate(currentPosition);

            foreach (var gridObject in gridObjects)
            {
                if (AddGridObject(newPosition, gridObject))
                {
                    RemoveGridObject(currentPosition, gridObject);
                }
            }
        }

        public void placeTiles(Vector2Int currentPosition, int range, Dictionary<Vector2Int, TileData> grid)
        {
            //todo show range of units
            //unfinished
            List <Vector2Int> x = allReachableTiles(currentPosition, range, grid);
        }

        public void MoveUnit(Vector2Int currentPosition, Vector2Int newPosition, GridObject gridObject, IUnit iUnit)
        {
            GameObject gameObject = iUnit.gameObject;
            TileData tileData = GetTileDataByCoordinate(newPosition);
            int moveRange = 4;
            //Check if in range
            if (tileData.GridObjects.Count == 0)
            {
                if (allReachableTiles(currentPosition, moveRange, tileDatas).Contains(newPosition))
                {
                    teleportUnit(currentPosition,newPosition,iUnit,gridObject);
                }
                else
                {
                    Debug.Log("Out of range");
                }
                
            }
            else
            {
                Debug.Log("Space occupied");
            }
        }

        public void teleportUnit(Vector2Int currentPosition, Vector2Int newPosition, IUnit unit, GridObject gridObject)
        {
            //Moves units gameobject along with the gridobject straight to a new position
            GameObject gameObject = unit.gameObject;
            gameObject.transform.position = ConvertCoordinateToPosition(newPosition);
            MoveGridObject(currentPosition, newPosition, gridObject);
        }

        public void MoveGridObject(Vector2Int currentPosition, Vector2Int newPosition, GridObject gridObject)
        {
            if (AddGridObject(newPosition, gridObject))
            {
                RemoveGridObject(currentPosition, gridObject);
            }
        }
        
        
        
        
        #endregion
    }
}