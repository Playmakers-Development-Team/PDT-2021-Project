using System;
using System.Collections.Generic;
using Units;
using UI;
using Abilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        private GridManager gridManager;

        private BoundsInt bounds;
        private Vector3 tilemapOriginPoint;

        private List<AbilityCard> abilityCards = new List<AbilityCard>();
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private Transform abilityParent;
        //private int Count => maxAbilities.Count;
        
        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.levelTilemap = GetComponentInChildren<Tilemap>();
            gridManager.InitialiseTileDatas();

            // NOTE: You can reset the bounds by going to Tilemap settings in the inspector and select "Compress Tilemap Bounds"
            bounds = gridManager.levelTilemap.cellBounds;
            tilemapOriginPoint = gridManager.levelTilemap.transform.position;
            
            //DrawGridOutline();
            TestingGetGridObjectsByCoordinate(0);
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0)) 
                ClickUnit();
        }

        #region Unit Selection

        private void ClickUnit()
        { 
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition - Camera.main.transform.position);
            Vector2 mousePos2D = new Vector2(mousePos.x + 0.5f, mousePos.y+0.5f);
            Vector2Int gridPos = gridManager.ConvertPositionToCoordinate(mousePos2D);
            PlayerManager playerManager = ManagerLocator.Get<PlayerManager>();
        
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
        }

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
        #endregion
    }
}
