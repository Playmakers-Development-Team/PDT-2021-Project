using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace UI.Refactored
{
    public class Grid : Element
    {
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Required Components")]
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private Button gridButton;
        
        private GridManager gridManager;
        
        
        protected override void OnAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();

            manager.gridSpacesSelected.AddListener(OnGridSelected);
            manager.gridSpacesDeselected.AddListener(OnGridDeselected);
        }

        private void Start()
        {
            FillAll();
        }

        private void FillAll()
        {
            Vector2Int bounds = gridManager.LevelBounds;
            for (int x = -bounds.x / 2 - 1; x <= bounds.x / 2; x++)
            {
                for (int y = -bounds.y / 2 - 1; y <= bounds.y / 2; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    tilemap.SetTile(position, defaultTile);
                }
            }
        }

        private void OnGridSelected(GridSelection selection)
        {
            TileBase tileBase = GetTile(selection.Type);

            foreach (Vector2Int coordinate in selection.Spaces)
            {
                Vector2Int bounds = gridManager.LevelBounds;
                if (coordinate.x < -bounds.x / 2 - 1 || coordinate.x > bounds.x / 2 || coordinate.y < -bounds.y / 2 - 1 ||
                    coordinate.y > bounds.y / 2)
                    continue;
                tilemap.SetTile(new Vector3Int(coordinate.x, coordinate.y, 0), tileBase);
            }
        }

        private TileBase GetTile(GridSelectionType type)
        {
            return type switch
            {
                GridSelectionType.Default => defaultTile,
                GridSelectionType.Invalid => invalidTile,
                GridSelectionType.Valid => validTile,
                GridSelectionType.Selected => selectedTile,
                _ => null
            };
        }

        private void OnGridDeselected()
        {
            FillAll();
        }

        public void OnGridButtonPressed()
        {
            if (!Camera.main)
                return;
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            manager.gridClicked.Invoke(new Vector2Int((int) worldPosition.x, (int) worldPosition.y));
        }
    }
}
