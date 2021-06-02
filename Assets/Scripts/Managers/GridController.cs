using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using GridObjects;
using Units;
using UI;
using Abilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utility;
using Managers;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] TileBase abilityHighlightTile;

        [SerializeField] Tilemap highlightTilemap;
        [SerializeField] Tilemap levelTilemap;

        private GridManager gridManager;
        private UIManager uiManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.levelTilemap = levelTilemap;
            uiManager = ManagerLocator.Get<UIManager>();

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
                if (ManagerLocator.Get<PlayerManager>().SelectedUnit != null)
                {
                    ClickCoordinateGrid();
                    ManagerLocator.Get<PlayerManager>().DeselectUnit();
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
            Vector3 mousePos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition -
                                               Camera.main.transform.position);
            Vector2 mousePos2D = new Vector2(mousePos.x + 0.5f, mousePos.y + 0.5f);
            Vector2Int gridPos = gridManager.ConvertPositionToCoordinate(mousePos2D);
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();

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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - Camera.main.transform.position);
            Vector2 mousePos2D = new Vector2(mousePos.x + 0.5f, mousePos.y + 0.5f);
            Vector2Int gridPos = gridManager.ConvertPositionToCoordinate(mousePos2D);
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            IUnit playerUnit = playerManager.SelectedUnit;

            Debug.Log(playerUnit.Coordinate + " to " + gridPos + " selected");
            List<GridObject> gridUnit = gridManager.GetGridObjectsByCoordinate(playerUnit.Coordinate);
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.MoveCommand(playerUnit, gridPos, playerUnit.Coordinate,
                    gridUnit.First()));
        }
        //
        // private void UpdateAbilityUI(PlayerUnit unit)
        // {
        //     ClearAbilityUI();
        //     
        //     if (unit is null)
        //     {
        //         Debug.LogWarning("GridController.UpdateAbilityUI should not be passed a null value. Use GridController.ClearAbilityUI instead.");
        //         return;
        //     }
        //     
        //     foreach (var ability in unit.GetAbilities())
        //     {
        //         AddAbilityField(ability);
        //     }
        //
        //     // TODO remove Test ability highlight
        //     TestAbilityHighlight(unit, unit.GetAbilities().First());
        // }
        // #endregion
        //
        // #region Abilities
        //
        // private void ClearAbilityUI()
        // {
        //     foreach (var abilityCard in abilityCards)
        //     {
        //         Destroy(abilityCard.gameObject);
        //     }
        //     
        //     abilityCards.Clear();
        // }
        //
        // private void AddAbilityField(Ability ability)
        // {
        //     var abilityCardObject = Instantiate(abilityUIPrefab, abilityParent);
        //     var abilityCard = abilityCardObject.GetComponent<AbilityCard>();
        //     abilityCard.SetAbility(ability);
        //     abilityCards.Add(abilityCard);
        // }
        //
        // private void HighlightAbility(Vector2Int originCoordinate, Vector2 targetVector, Ability ability)
        // {
        //     ClearAbilityHighlight();
        //     var highlightedCoordinates = ability.Shape.GetHighlightedCoordinates(originCoordinate, targetVector);
        //
        //     foreach (Vector2Int highlightedCoordinate in highlightedCoordinates)
        //     {
        //         highlightTilemap.SetTile((Vector3Int) highlightedCoordinate, abilityHighlightTile);
        //     }
        // }



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
