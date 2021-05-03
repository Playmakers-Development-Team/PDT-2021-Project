using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        private Tilemap tilemap;
        private GridManager gridManager;

        private void Awake()
        {
            tilemap = GetComponentInChildren<Tilemap>();
            
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.InitialiseTileDatas(tilemap);
        
            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            BoundsInt bounds = tilemap.cellBounds;
            DrawGridOutline(bounds);
            TestingGetGridObjectsByCoordinate(1, bounds);
        }

        #region Function Testing
        
        // DrawGridOutline shows the size of the grid in the scene view based on tilemap.cellBounds
        private void DrawGridOutline(BoundsInt bounds)
        {
            Vector3[] gridCorners = {
                new Vector3(bounds.xMin, bounds.yMin, 0),
                new Vector3(bounds.xMax, bounds.yMin, 0),
                new Vector3(bounds.xMax, bounds.yMax, 0),
                new Vector3(bounds.xMin, bounds.yMax, 0)
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
        
        private void TestingGetGridObjectsByCoordinate(int testCases, BoundsInt bounds)
        {
            for (int i = 0; i < testCases; i++)
            {
                Vector2 randomCoordinates = new Vector2(Random.Range(0, bounds.size.x), Random.Range(0, bounds.size.y));
                TileBase tile = gridManager.GetGridObjectsByCoordinate(
                    randomCoordinates.x,
                    randomCoordinates.y
                ).Tile;
                print(tile + " is at the provided coordinates " + randomCoordinates);
            }
        }

        #endregion
    }
}
