using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Vector2Int levelBounds;
        [SerializeField] private Tilemap levelTilemap;

        private GridManager gridManager;

        
        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.InitialiseGrid(levelTilemap, levelBounds);
            
            TestingGetGridObjectsByCoordinate(0);
        }

        
        #region Unit Testing
        
        private void TestingGetGridObjectsByCoordinate(int testCases)
        {
            for (int i = 0; i < testCases; i++)
            {
                Vector2Int randomCoordinates = gridManager.GetRandomCoordinates();

                TileBase tile = gridManager.GetTileDataByCoordinate(
                        new Vector2Int(randomCoordinates.x, randomCoordinates.y)).
                    Tile;
                print(tile + " is at the provided coordinates " + randomCoordinates);
            }
        }

        #endregion
        
        
        #region Visualisation
        
#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Handles.color = Color.green;
            BoundsInt b = new BoundsInt(
                new Vector3Int(-Mathf.FloorToInt(levelBounds.x / 2.0f), -Mathf.FloorToInt(levelBounds.y / 2.0f), 0),
                new Vector3Int(levelBounds.x, levelBounds.y, 0)
            );
            
            for (int x = b.xMin; x <= b.xMax; x++)
            {
                Vector3 start = levelTilemap.CellToWorld(new Vector3Int(x, b.yMin, 0));
                Vector3 end = levelTilemap.CellToWorld(new Vector3Int(x, b.yMax, 0));
            
                if (x == b.xMin || x == b.xMax)
                    Handles.DrawLine(start, end, 5.0f);
                else
                    Handles.DrawDottedLine(start, end, 5.0f);
            }
            
            for (int y = b.yMin; y <= b.yMax; y++)
            {
                Vector3 start = levelTilemap.CellToWorld(new Vector3Int(b.xMin, y, 0));
                Vector3 end = levelTilemap.CellToWorld(new Vector3Int(b.xMax, y, 0));
            
                if (y == b.yMin || y == b.yMax)
                    Handles.DrawLine(start, end, 5.0f);
                else
                    Handles.DrawDottedLine(start, end, 5.0f);
            }
        }
        
#endif
        
        #endregion
    }
}
