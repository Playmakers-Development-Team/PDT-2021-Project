using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : Manager
    {
        private Dictionary<Vector2, TileData> tileDatas = new Dictionary<Vector2, TileData>();

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
                        
                        tileDatas.Add(new Vector2(x, y), new TileData(tile));

                        // Debug.Log(tileData.Tile.name + " is at position " + tileData.Position);
                    }
                }
            }
        }
        
        public TileData GetGridObjectsByCoordinate(float x, float z)
        {
            Vector2 coordinate = new Vector2(x, z);
            
            if(tileDatas.TryGetValue(coordinate, out TileData tileData))
            {
                return tileData;
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + coordinate);
            return null;
        }
    }
}