using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using TileData = Tiles.TileData;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        [SerializeField] private List<TileData> tileDatas = new List<TileData>();

        private Grid grid { get; set; }

        private void Awake()
        {
            grid = GetComponent<Grid>();
            ManagerLocator.Get<GridManager>().Controller = this;
            tilemap = GetComponentInChildren<Tilemap>();
            
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
            
            TestingGetGridObjectsByCoordinate(10);
            
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

        #region Function Testing
        
        private void TestingGetGridObjectsByCoordinate(float successfulCases)
        {
            for (int i = 0; i < successfulCases; i++)
            {
                Vector2 randomCoordinates = new Vector2(Random.Range(2,18), Random.Range(0,12));
                print(GetGridObjectsByCoordinate(randomCoordinates.x, randomCoordinates.y) + " is at the provided coordinates " + randomCoordinates);
            }
            
            // print(GetGridObjectsByCoordinate(1, -1) + "Error should return since coordinates are out of bounds");
            // print(GetGridObjectsByCoordinate(18, 12) + "Error should return since coordinates are out of bounds");
        }
        
        #endregion
    }
}
