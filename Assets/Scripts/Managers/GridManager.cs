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

            return null;
        }
        
        public int[,] getUnitRange(int[,] gridArray, int moveRange, Vector2Int initialPos){
            
            Queue<Vector2Int> coordQueue = new Queue<Vector2Int>();
            coordQueue.Enqueue(initialPos);
            
            while (coordQueue.Count > 0)
            {
                Vector2Int current = coordQueue.Peek();
                int currentMoveCount = gridArray[current.x, current.y];
                if(currentMoveCount == moveRange){coordQueue.Clear(); break;}
                //mark adjacent grids and add them to the back of the queue
                //Only mark if 0
                //Implement Method to increase maintainability
                if (gridArray[current.x + 1, current.y] == 0)
                {
                    if (GetGridObjectsByCoordinate(current) != null)// will need to be updated to allow allies through
                    {
                        gridArray[current.x + 1, current.y] = currentMoveCount + 1;
                        coordQueue.Enqueue(new Vector2Int(current.x + 1, current.y)); //right 
                    }
                }
                if (gridArray[current.x, current.y - 1] == 0)
                {
                    if (GetGridObjectsByCoordinate(current) != null) 
                    {
                        gridArray[current.x, current.y - 1] = currentMoveCount + 1;
                        coordQueue.Enqueue(new Vector2Int(current.x, current.y - 1)); //down
                    }
                }
                if (gridArray[current.x - 1, current.y] == 0)
                {
                    if (GetGridObjectsByCoordinate(current) != null)
                    {
                        gridArray[current.x - 1, current.y] = currentMoveCount + 1;
                        coordQueue.Enqueue(new Vector2Int(current.x - 1, current.y)); //left
                    }
                }
                if (gridArray[current.x, current.y + 1] == 0)
                {
                    if (GetGridObjectsByCoordinate(current) != null)
                    {
                        gridArray[current.x, current.y + 1] = currentMoveCount + 1;
                        coordQueue.Enqueue(new Vector2Int(current.x, current.y + 1)); //up 
                    }
                }
                coordQueue.Dequeue();
                //repeat until queue empty
            }
            return gridArray;
        }
        public Queue<Vector2Int> getUnitPath(int[,] gridArray, Vector2Int targetPos){
            
            Queue<Vector2Int> coordQueue = new Queue<Vector2Int>();
            Queue<Vector2Int> pathQueue = new Queue<Vector2Int>();
            coordQueue.Enqueue(targetPos);
            
            while (coordQueue.Count > 0)
            {
                Vector2Int current = coordQueue.Peek();
                int currentMoveCount = gridArray[current.x, current.y];
                if (gridArray[current.x + 1, current.y] == currentMoveCount - 1)
                {
                    coordQueue.Enqueue(new Vector2Int(current.x + 1, current.y)); //right 
                }else
                if (gridArray[current.x, current.y - 1] == currentMoveCount - 1)
                {
                    coordQueue.Enqueue(new Vector2Int(current.x, current.y - 1)); //down
                }else
                if (gridArray[current.x - 1, current.y] == currentMoveCount - 1)
                {
                    coordQueue.Enqueue(new Vector2Int(current.x - 1, current.y)); //left
                }else
                if (gridArray[current.x, current.y + 1]== currentMoveCount - 1)
                {
                    coordQueue.Enqueue(new Vector2Int(current.x, current.y + 1)); //up 
                }
                pathQueue.Enqueue(current);
                coordQueue.Dequeue();
                //repeat until queue empty
            }
            return pathQueue;
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

        public void MoveUnit(Vector2Int currentPosition, Vector2Int newPosition, GridObject gridObject, IUnit iUnit)
        {
            GameObject gameObject = iUnit.gameObject;
            TileData tileData = GetTileDataByCoordinate(newPosition);

            if (tileData.GridObjects.Count == 0)
            {
                gameObject.transform.position = ConvertCoordinateToPosition(newPosition);
                MoveGridObject(currentPosition, newPosition, gridObject);
            }
            else
            {
                Debug.Log("Space occupied");
            }
        }

        public void MoveGridObject(Vector2Int currentPosition, Vector2Int newPosition, GridObject gridObject)
        {
            if (AddGridObject(newPosition, gridObject))
            {
                RemoveGridObject(currentPosition, gridObject);
            }
        }

        public void CheckIfInRange()
        {
            //if(coords not 0 in grid array)
            
            //MoveGridObject()
        }
        
        
        
        #endregion
    }
}