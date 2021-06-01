using System;
using System.Collections.Generic;
using System.Linq;
using Units;
using UI;
using Abilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utility;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Vector2Int levelBounds;
        [SerializeField] private Vector2 gridOffset;
        [SerializeField] private Tilemap levelTilemap;
        [SerializeField] private Tilemap highlightTilemap;
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private Transform abilityParent;
        [SerializeField] private TileBase abilityHighlightTile;

        private GridManager gridManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private List<AbilityCard> abilityCards = new List<AbilityCard>();
        //private int Count => maxAbilities.Count;
        
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
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0)) 
                ClickUnit();
            
            DrawGridOutline();
        }

        #region Unit Selection

        private void ClickUnit()
        { 
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = gridManager.ConvertPositionToCoordinate(mousePos);
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
            Vector2Int actualGridPos = (Vector2Int) levelTilemap.layoutGrid.WorldToCell(mousePos);
            
            Debug.Log($"Click mouse grid {gridPos} actually {actualGridPos} at mouse pos {mousePos}");
        
            foreach (IUnit unit in playerManager.PlayerUnits)
            {
                if (unit is PlayerUnit playerUnit)
                {
                    if (gridManager.ConvertPositionToCoordinate(playerUnit.transform.position) == gridPos)
                    {
                        playerManager.SelectUnit(playerUnit);
                        UpdateAbilityUI(playerUnit);
                        Debug.Log($"Unit Selected!");
                        return;
                    }
                }
            }
            playerManager.DeselectUnit();
            ClearAbilityHighlight();
            ClearAbilityUI();
            Debug.Log($"Unit Deselected!");
        }

        private void UpdateAbilityUI(PlayerUnit unit)
        {
            ClearAbilityUI();
            
            if (unit is null)
            {
                Debug.LogWarning("GridController.UpdateAbilityUI should not be passed a null value. Use GridController.ClearAbilityUI instead.");
                return;
            }
            
            foreach (var ability in unit.GetAbilities())
            {
                AddAbilityField(ability);
            }

            // TODO remove Test ability highlight
            TestAbilityHighlight(unit, unit.GetAbilities().First());
        }
        #endregion
        
        #region Abilities

        private void ClearAbilityUI()
        {
            foreach (var abilityCard in abilityCards)
            {
                Destroy(abilityCard.gameObject);
            }
            
            abilityCards.Clear();
        }

        private void AddAbilityField(Ability ability)
        {
            var abilityCardObject = Instantiate(abilityUIPrefab, abilityParent);
            var abilityCard = abilityCardObject.GetComponent<AbilityCard>();
            abilityCard.SetAbility(ability);
            abilityCards.Add(abilityCard);
        }

        private void HighlightAbility(Vector2Int originCoordinate, Vector2 targetVector, Ability ability)
        {
            ClearAbilityHighlight();
            var highlightedCoordinates = ability.Shape.GetHighlightedCoordinates(originCoordinate, targetVector);

            foreach (Vector2Int highlightedCoordinate in highlightedCoordinates)
            {
                highlightTilemap.SetTile((Vector3Int) highlightedCoordinate, abilityHighlightTile);
            }
        }

        private void ClearAbilityHighlight()
        {
            highlightTilemap.ClearAllTiles();
        }

        #endregion

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

        private void TestAbilityHighlight(IUnit unit, Ability ability)
        {
            HighlightAbility(unit.Coordinate,
            ((OrdinalDirection) UnityEngine.Random.Range(0,
                Enum.GetValues(typeof(OrdinalDirection)).Length)).ToVector2(), ability);
        }
        
        #endregion
    }
}
