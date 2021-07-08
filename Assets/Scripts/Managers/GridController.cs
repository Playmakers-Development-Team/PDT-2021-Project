using System;
using System.Collections.Generic;
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
        [SerializeField] private bool drawGridOutline = false;

        private GridManager gridManager;
        
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.InitialiseGrid(levelTilemap, levelBounds, gridOffset);
            
            tilemapOriginPoint = levelTilemap.transform.position;
            
            if(drawGridOutline) DrawGridOutline();
            TestingGetGridObjectsByCoordinate(0);
        }

        #region Unit Testing

        // DrawGridOutline shows the size of the grid in the scene view based on tilemap.cellBounds
        private void DrawGridOutline()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            
            Vector2[] gridCorners =
            {
                gridManager.ConvertCoordinateToPosition(levelBounds),
                gridManager.ConvertCoordinateToPosition(new Vector2Int(levelBounds.x, 0)),
                gridManager.ConvertCoordinateToPosition(Vector2Int.zero),
                gridManager.ConvertCoordinateToPosition(new Vector2Int(0, levelBounds.y)),
                gridManager.ConvertCoordinateToPosition(levelBounds)
            };
            
            for (int i = 0; i < gridCorners.Length; i++)
            {
                // NOTE: Need to shift positions down (on the y-axis) to line up with the grid 
                lineRenderer.SetPosition(i, gridCorners[i] - levelTilemap.cellSize * Vector2.up);
            }
            
            Debug.Log("Draw Grid Outline is on. To turn it off go to the grid in inspector" +
                      "and uncheck the option");
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
