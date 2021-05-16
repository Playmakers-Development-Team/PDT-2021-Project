using System.Collections.Generic;
using System.ComponentModel;
using GridObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : IManager
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
                return new List<GridObject>();

            return tileData.GridObjects;
        }
        
        #endregion
        
        public Vector2Int ConvertWorldSpaceToGridSpace(Vector2 worldSpace)
        {
            Debug.Log("WorldSpace: " + worldSpace + " | GridSpace: " + 
                      (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace));
            return (Vector2Int) levelTilemap.layoutGrid.WorldToCell(worldSpace);
        }
        
        #region GRID OBJECT FUNCTIONS

        public bool AddGridObject(Vector2Int gridPosition, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(gridPosition);

            if (!(tileData is null))
            {
                Debug.Log("GridObject added to tile " + gridPosition.x + ", " + gridPosition.y);
                tileData.AddGridObjects(gridObject);
                return true;
            }

            Debug.LogWarning("Failed to add grid object at " + gridPosition.x + ", " + gridPosition.y +
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