using System;
using System.Collections.Generic;
using Commands;
using GridObjects;
using Units;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace Managers
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Vector2Int levelBounds;
        [SerializeField] private Vector2 gridOffset;
        [SerializeField] private Tilemap levelTilemap;

        private GridManager gridManager;
        
        private Vector3 tilemapOriginPoint;

        private void Awake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            gridManager.InitialiseGrid(levelTilemap, levelBounds, gridOffset);
            
            tilemapOriginPoint = levelTilemap.transform.position;
            
            TestingGetGridObjectsByCoordinate(0);
        }

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

            if (!levelTilemap)
                return;

            Handles.color = Color.red;
            Handles.zTest = CompareFunction.Less;
            foreach (Transform child in levelTilemap.transform)
            {
                if (!child.GetComponent<Obstacle>())
                    continue;
                
                Vector3Int cellPosition = levelTilemap.WorldToCell(child.position);
                
                Vector3[] positions =
                {
                    levelTilemap.CellToWorld(cellPosition),
                    levelTilemap.CellToWorld(cellPosition + new Vector3Int(0, 1, 0)),
                    levelTilemap.CellToWorld(cellPosition + new Vector3Int(1, 1, 0)),
                    levelTilemap.CellToWorld(cellPosition + new Vector3Int(1, 0, 0))
                };
                int[] indices = {0, 1, 1, 2, 2, 3, 3, 0, 0, 2, 1, 3};

                for (int i = 0; i < indices.Length; i += 2)
                    Handles.DrawLine(positions[indices[i]], positions[indices[i + 1]], 5.0f);
            }
        }
        
#endif

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
    }
}
