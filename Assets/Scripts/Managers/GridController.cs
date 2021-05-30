using TMPro;
using System;
using System.Collections.Generic;
using Units;
using StatusEffects;
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

        private List<TextMeshProUGUI> abilityUIText;
        [SerializeField] private GameObject abilityUIPrefab;
        [SerializeField] private GameObject abilityParent;

        private int Count => abilityUIText.Count;
        
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
                    if (gridManager.ConvertWorldSpaceToGridSpace(playerUnit.transform.position) == gridPos)
                    {
                        playerManager.SelectUnit(playerUnit);
                        UpdateAbility(playerUnit);
                        Debug.Log($"Unit Selected!");
                        return;
                    }
                }
            }
            playerManager.SelectUnit(null);
        }

        private void UpdateAbility(PlayerUnit unit)
        {
            int i = 0;
            foreach (TenetStatusEffect s in unit.TenetStatusEffects)
            {
                if(i > Count) AddAbilityField(s.TenetType.ToString());
                else SetAbilityText(i,s.TenetType.ToString());
                //Debug.Log(s);
                i++;
            }
        }

        public void AddAbilityField(string abilityName)
        {
            GameObject abilityUI = Instantiate(abilityUIPrefab, transform);
        }

        private void SetAbilityText(int index, string abilityName)
        {
            abilityUIText[index].text = abilityName;
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
