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
        private Dictionary<Vector2, TileData> tileDatas = new Dictionary<Vector2, TileData>();

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
                        // TODO: Add a way to detect gridObjects before tileDatas.Add
                        
                        tileDatas.Add(new Vector2(x, y), new TileData(tile));

                        // Debug.Log(tileData.Tile.name + " is at position " + tileData.Position);
                    }
                }
            }
        }
        
        public TileData GetTileDataByCoordinate(Vector2 coordinate)
        {
            if(tileDatas.TryGetValue(coordinate, out TileData tileData))
            {
                return tileData;
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + coordinate);
            return null;
        }
        
        public List<GridObject> GetGridObjectsByCoordinate(Vector2 coordinate)
        {
            TileData tileData = GetTileDataByCoordinate(coordinate);

            if (tileData is null)
                return new List<GridObject>();

            return tileData.GridObjects;
        }

        public Vector3 ConvertWorldSpaceToGridSpace(Vector3 worldSpace)
        {
            Debug.Log("WorldSpace: " + worldSpace + " | GridSpace: " + levelTilemap.layoutGrid.WorldToCell(worldSpace));
            return levelTilemap.layoutGrid.WorldToCell(worldSpace);
        }

        public void AddGridObject(Vector2Int position, GridObject gridObject)
        {
            TileData tileData = GetTileDataByCoordinate(position);

            if (!(tileData is null))
            {
                Debug.Log("GridObject added to tile " + position.x + ", " + position.y);
                tileData.AddGridObjects(gridObject);
            }
        }
    }
}