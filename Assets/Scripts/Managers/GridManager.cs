using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridManager : IManager
    {
        public GridController Controller { get; set; }
        
        private List<TileData> tileDatas = new List<TileData>();

        public void Initialise(Tilemap tilemap)
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
                        TileData tileData = new TileData(tile, new Vector2(x, y));
                        tileDatas.Add(tileData);
                        //Debug.Log(tileData.Tile.name + " is at position " + tileData.Position);
                    }
                }
            }
        }
        
        public TileBase GetGridObjectsByCoordinate(float x, float z)
        {
            Vector2 coordinate = new Vector2(x, z);
            foreach (TileData tileData in tileDatas)
            {
                if (tileData.Position == coordinate)
                {
                    return tileData.Tile;
                }
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + coordinate);
            return null;
        }
    }
}