using System.Collections.Generic;
using GridObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : IManager
    {
        public GridController Controller { get; set; }
        
        private Dictionary<Vector2Int, TileData> tileDatas = new Dictionary<Vector2Int, TileData>();

        public void InitialiseTileDatas(Tilemap tilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];

                    if (tile != null)
                    {
                        // TODO: Add a way to detect gridObjects before tileDatas.Add

                        tileDatas.Add(new Vector2Int(x, y), new TileData(tile));

                        // Debug.Log(tileData.Tile.name + " is at position " + tileData.Position);
                    }
                }
            }
        }

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