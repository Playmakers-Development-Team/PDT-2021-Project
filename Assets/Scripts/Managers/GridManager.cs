using System.Collections.Generic;
using GridObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : Manager
    {
        private Dictionary<Vector2Int, TileData> tileDatas = new Dictionary<Vector2Int, TileData>();

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
        
        public TileData GetTileDataByCoordinate(Vector2Int gridPosition)
        {
            if(tileDatas.TryGetValue(gridPosition, out TileData tileData))
            {
                return tileData;
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + gridPosition);
            return null;
        }
        
        public List<GridObject> GetGridObjectsByCoordinate(Vector2Int gridPosition)
        {
            TileData tileData = GetTileDataByCoordinate(gridPosition);

            if (tileData is null)
            {
                Debug.LogError("ERROR: No tileData was found for the provided coordinates " + gridPosition);
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
            Vector2Int coordinates = GetRandomCoordinates();
            
            while (GetGridObjectsByCoordinate(coordinates).Count > 0)
            {
                coordinates = GetRandomCoordinates();
            }
            
            return coordinates;
        }
        
        #endregion

        #region CONVERSIONS
        
        public Vector2Int ConvertWorldSpaceToGridSpace(Vector2 worldSpace)
        {
            // Debug.Log("WorldSpace: " + worldSpace + " | GridSpace: " + 
            //           (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace));
            return (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace);
        }
        
        public Vector2 ConvertGridSpaceToWorldSpace(Vector2Int gridSpace)
        {
            // Debug.Log("GridSpace: " + gridSpace + " | WorldSpace: " + 
            //           levelTilemap.layoutGrid.CellToWorld((Vector3Int) gridSpace));
            return levelTilemap.layoutGrid.CellToWorld((Vector3Int) gridSpace);
        }
        
        #endregion
        
        #region GRID OBJECT FUNCTIONS

        public bool AddGridObject(Vector2Int gridPosition, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(gridPosition);
            
            if (tileData != null)
            {
                if (tileData.GridObjects.Count > 0)
                {
                    // Can change this later if we want to allow multiple grid objects on a tile
                    Debug.LogWarning("Failed to add " + gridObject +
                                     " at " + gridPosition.x + ", " + gridPosition.y +
                                     " due to tile being occupied by " + tileData.GridObjects[0]);
                    return false;
                }

                Debug.Log(gridObject + " added to tile " + gridPosition.x + ", " + gridPosition.y);
                tileData.AddGridObjects(gridObject);
                return true;
            }
            
            Debug.LogWarning("Failed to add " + gridObject + 
                             " at " + gridPosition.x + ", " + gridPosition.y +
                             " due to null tileData");
            
            return false;
        }
        
        public bool RemoveGridObject(Vector2Int gridPosition, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(gridPosition);

            if (tileData.GridObjects.Contains(gridObject))
            {
                Debug.Log("GridObject removed from tile " + gridPosition.x + ", " + gridPosition.y);
                tileData.RemoveGridObjects(gridObject);
                return true;
            }
            
            Debug.LogWarning("Failed to remove gridObject at " + gridPosition.x + ", " + gridPosition.y + 
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