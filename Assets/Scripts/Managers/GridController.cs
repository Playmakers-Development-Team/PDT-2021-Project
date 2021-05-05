using Commands;
using Commands.Shapes;
using GridObjects;
using Units;
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
            gridManager.Controller = this;
            gridManager.InitialiseTileDatas(tilemap);
        
            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            BoundsInt bounds = tilemap.cellBounds;
            DrawGridOutline(bounds);
            TestingGetGridObjectsByCoordinate(1, bounds);
            TestAbility();
        }

        #region Function Testing
        
        // DrawGridOutline shows the size of the grid in the scene view based on tilemap.cellBounds
        private void DrawGridOutline(BoundsInt bounds)
        {
            Vector3[] gridCorners = {
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
        
        private void TestingGetGridObjectsByCoordinate(int testCases, BoundsInt bounds)
        {
            for (int i = 0; i < testCases; i++)
            {
                Vector2Int randomCoordinates = new Vector2Int(
                    Random.Range(0, bounds.size.x),
                    Random.Range(0, bounds.size.y)
                );
                TileBase tile = gridManager.GetTileDataByCoordinate(randomCoordinates).Tile;
                print(tile + " is at the provided coordinates " + randomCoordinates);
            }
        }

        private void TestAbility()
        {
            Unit unit = new Unit(
                Vector2Int.one,
                new TakeWeightedDamageBehaviour(2),
                new TakeWeightedKnockbackBehaviour(0)
            );
            
            Shape shape = new Point(Vector2Int.one);

            AbilityCommand ability = new AbilityCommand(unit, shape, 4, 2);
            
            ability.Execute(); // Unit should take 8 damage and 0 knockback
        }

        #endregion
    }
}
