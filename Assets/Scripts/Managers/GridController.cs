using System.Collections.Generic;
using System.Linq;
using GridObjects;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private TileBase abilityHighlightTile;

        [SerializeField] private Tilemap highlightTilemap;
        [SerializeField] private Tilemap levelTilemap;

        private GridManager gridManager;
        private UIManager uiManager;
        private PlayerManager playerManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.levelTilemap = levelTilemap;
            uiManager = ManagerLocator.Get<UIManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();

            uiManager.Initialise(abilityHighlightTile, highlightTilemap);
            gridManager.InitialiseTileDatas();

            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            bounds = gridManager.levelTilemap.cellBounds;
            tilemapOriginPoint = gridManager.levelTilemap.transform.position;

            //DrawGridOutline();
            TestingGetGridObjectsByCoordinate(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (playerManager.SelectedUnit != null)
                {
                    ClickCoordinateGrid();
                    playerManager.DeselectUnit();
                    Debug.Log($"Unit Deselected!");
                }
                else
                {
                    ClickUnit();
                }
            }
        }

        #region Unit Selection

        private void ClickUnit()
        {
            Vector2Int gridPos = GetCoordinateFromClick();

            foreach (IUnit unit in playerManager.PlayerUnits)
            {
                if (unit is PlayerUnit playerUnit)
                {
                    if (gridManager.ConvertPositionToCoordinate(playerUnit.transform.position) ==
                        gridPos)
                    {
                        playerManager.SelectUnit(playerUnit);
                        //UpdateAbilityUI(playerUnit);
                        Debug.Log($"Unit Selected!");
                        return;
                    }
                }
            }
            
            playerManager.DeselectUnit();
            Debug.Log($"Unit Deselected!");
            // ClearAbilityUI();
        }

        private void ClickCoordinateGrid()
        {
            Vector2Int gridPos = GetCoordinateFromClick();
            
            IUnit playerUnit = playerManager.SelectedUnit;

            Debug.Log(playerUnit.Coordinate + " to " + gridPos + " selected");
            List<GridObject> gridUnit = gridManager.GetGridObjectsByCoordinate(playerUnit.Coordinate);
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.MoveCommand(playerUnit, gridPos, playerUnit.Coordinate,
                    gridUnit.First()));
        }

        private Vector2Int GetCoordinateFromClick()
        {
            Vector3 mousePosScreenSpace = Input.mousePosition - Camera.main.transform.position;
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(mousePosScreenSpace);
            Vector2 mousePos2D = new Vector2(mousePosWorldSpace.x + 0.5f, mousePosWorldSpace.y + 0.5f);
            return gridManager.ConvertPositionToCoordinate(mousePos2D);
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
