using System;
using Commands;
using GridObjects;
using Units;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Vector2Int levelBounds;
        [SerializeField] private Vector2 gridOffset;
        [SerializeField] private Tilemap levelTilemap;

        private GridManager gridManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.InitialiseGrid(levelTilemap, levelBounds, gridOffset);

            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            bounds = gridManager.LevelTilemap.cellBounds;
            tilemapOriginPoint = gridManager.LevelTilemap.transform.position;

            //DrawGridOutline();
            TestingGetGridObjectsByCoordinate(0);
        }

        #region Unit Selection

        private void ClickUnit()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // TODO look into this later, put the subtraction somewhere better
            Vector2Int gridPos = gridManager.ConvertPositionToCoordinate(mousePos) + new Vector2Int(1, 1);
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();

            foreach (IUnit unit in playerManager.PlayerUnits)
            {
                if (unit is PlayerUnit playerUnit)
                {
                    if (gridManager.ConvertPositionToCoordinate(playerUnit.transform.position) ==
                        gridPos)
                    {
                        // TODO: Dependency Violation - Grid system should not depend on Unit system
                        playerManager.SelectUnit(playerUnit);
                        //UpdateAbilityUI(playerUnit);
                        Debug.Log($"Unit Selected!");
                        return;
                    }
                }
            }

            playerManager.DeselectUnit();
        }

        #endregion

        #region Unit Testing

        // DrawGridOutline shows the size of the grid in the scene view based on tilemap.cellBounds
        private void DrawGridOutline()
        {
            Vector3[] gridCorners =
            {
                new Vector3(bounds.xMin + tilemapOriginPoint.x,
                    bounds.yMin + tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMax + tilemapOriginPoint.x,
                    bounds.yMin + tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMax + tilemapOriginPoint.x,
                    bounds.yMax + tilemapOriginPoint.y, 0),
                new Vector3(bounds.xMin + tilemapOriginPoint.x,
                    bounds.yMax + tilemapOriginPoint.y, 0)
            };

            for (int i = 0; i < gridCorners.Length; i++)
            {
                if (i == gridCorners.Length - 1)
                {
                    Debug.DrawLine(gridCorners[i], gridCorners[0], Color.green, float.MaxValue);
                }
                else
                {
                    Debug.DrawLine(gridCorners[i], gridCorners[i + 1], Color.green, float.MaxValue);
                }
            }
        }

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
    }
}
