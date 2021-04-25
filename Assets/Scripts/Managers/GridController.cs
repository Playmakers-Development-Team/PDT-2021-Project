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

        private Grid grid;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            ManagerLocator.Get<GridManager>().Controller = this;
            tilemap = GetComponentInChildren<Tilemap>();

            InitializeTileDatas();
        }

        private void InitializeTileDatas()
        {
            //NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
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
                        
                        //TODO: Add items on the tile and add them to tile data
                        
                        tileDatas.Add(tileData);
                        
                        //Debug.Log(tileData.Tile.name + " is at position " + tileData.Position);
                    }
                }
            }
            
            DrawGridOutline(bounds);
            TestingGetGridObjectsByCoordinate(100, bounds);
        }
        
        public TileData GetGridObjectsByCoordinate(float x, float z)
        {
            Vector2 coordinate = new Vector2(x, z);
            foreach (TileData tileData in tileDatas)
            {
                if (tileData.Position == coordinate)
                {
                    return tileData;
                }
            }

            Debug.LogError("ERROR: No tile was found for the provided coordinates " + coordinate);
            return null;
        }

        #region Function Testing

        private void DrawGridOutline(BoundsInt bounds)
        {
            Vector3[] gridCorners =  new Vector3[]
            {
                new Vector3(bounds.xMin, 0, bounds.yMin),
                new Vector3(bounds.xMax, 0, bounds.yMin),
                new Vector3(bounds.xMax, 0, bounds.yMax),
                new Vector3(bounds.xMin, 0, bounds.yMax)
            };

            for (int i = 0; i < gridCorners.Length ; i++)
            {
                if (i == gridCorners.Length - 1)
                {
                    Debug.DrawLine(gridCorners[i], gridCorners[0], Color.green, float.MaxValue);
                }
                else
                {
                    Debug.DrawLine(gridCorners[i], gridCorners[i+1], Color.green, float.MaxValue);
                }
            }

        }
        
        private void TestingGetGridObjectsByCoordinate(float successfulCases, BoundsInt bounds)
        {
            for (int i = 0; i < successfulCases; i++)
            {
                Vector2 randomCoordinates = new Vector2(Random.Range(0, bounds.size.x), Random.Range(0, bounds.size.y));
                print(GetGridObjectsByCoordinate(randomCoordinates.x, randomCoordinates.y).Tile + " is at the provided coordinates " + randomCoordinates);
            }
        }

        #endregion
    }
}
