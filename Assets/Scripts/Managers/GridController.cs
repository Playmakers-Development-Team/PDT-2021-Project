using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

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
            gridManager.Controller = this;
            gridManager.Initialise(tilemap);
            
            TestingGetGridObjectsByCoordinate(10);
        }

        #region Function Testing
        
        private void TestingGetGridObjectsByCoordinate(int testCases)
        {
            for (int i = 0; i < testCases; i++)
            {
                Vector2 randomCoordinates = new Vector2(Random.Range(2,18), Random.Range(0,12));
                TileBase tile = gridManager.GetGridObjectsByCoordinate(
                    randomCoordinates.x,
                    randomCoordinates.y
                );
                print(tile + " is at the provided coordinates " + randomCoordinates);
            }
            
            // print(GetGridObjectsByCoordinate(1, -1) + "Error should return since coordinates are out of bounds");
            // print(GetGridObjectsByCoordinate(18, 12) + "Error should return since coordinates are out of bounds");
        }
        
        #endregion
    }
}
