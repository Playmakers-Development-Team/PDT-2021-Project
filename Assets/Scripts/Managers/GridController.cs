using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        private GridManager gridManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.levelTilemap = GetComponentInChildren<Tilemap>();
            gridManager.InitialiseTileDatas();

            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            bounds = gridManager.levelTilemap.cellBounds;
            tilemapOriginPoint = gridManager.levelTilemap.transform.position;
            
            DrawGridOutline();
            TestingGetGridObjectsByCoordinate(0);
        }

        #region Unit Testing
        
        // DrawGridOutline shows the size of the grid in the scene view based on tilemap.cellBounds
        private void DrawGridOutline()
        {
            Vector3[] gridCorners = {
                new Vector3(bounds.xMin + tilemapOriginPoint.x, bounds.yMin + tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMax + tilemapOriginPoint.x, bounds.yMin+ tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMax + tilemapOriginPoint.x, bounds.yMax+ tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMin + tilemapOriginPoint.x, bounds.yMax+ tilemapOriginPoint.y, 0)
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
        
        private void TestingGetGridObjectsByCoordinate(int testCases)
        {
            for (int i = 0; i < testCases; i++)
            {
                Vector2Int randomCoordinates = gridManager.GetRandomCoordinates();
                
                TileBase tile = gridManager.GetTileDataByCoordinate(
                    new Vector2Int(randomCoordinates.x, randomCoordinates.y)).Tile;
                print(tile + " is at the provided coordinates " + randomCoordinates);
            }
        }

        #endregion
    }
}
