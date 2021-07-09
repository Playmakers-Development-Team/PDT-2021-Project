using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UI
{
    public class GridUI : Element
    {
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Required Components")]
        
        [SerializeField] private Tilemap tilemap;
        
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
            Vector2Int levelBounds = gridManager.LevelBounds;
            for (int x = -levelBounds.x / 2; x <= levelBounds.x / 2; x++)
            {
                for (int y = -levelBounds.y / 2; y <= levelBounds.y / 2; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    tilemap.SetTile(position, defaultTile);
                }
            }
        }

        private void OnGridSelected(GridSelection selection)
        {
            TileBase tileBase = GetTile(selection.Type);

            foreach (Vector2Int correctCoordinate in selection.Spaces)
            {
                Vector2Int scuffedCoordinate = correctCoordinate;
                
                Vector2Int bounds = gridManager.LevelBounds;
                if (scuffedCoordinate.x < -bounds.x / 2 || scuffedCoordinate.x > bounds.x / 2 || scuffedCoordinate.y < -bounds.y / 2 ||
                    scuffedCoordinate.y > bounds.y / 2)
                    continue;
                
                tilemap.SetTile(new Vector3Int(scuffedCoordinate.x, scuffedCoordinate.y, 0), tileBase);
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
            if (Camera.main == null)
                return;
            
            Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(-Camera.main.transform.forward, transform.position);
            
            if (!plane.Raycast(worldRay, out float distance))
                return;
            
            Vector2 worldPosition = worldRay.origin + worldRay.direction * distance;
            Vector2Int coordinate = gridManager.ConvertPositionToCoordinate(worldPosition);

            manager.gridClicked.Invoke(coordinate);
        }
    }
}
