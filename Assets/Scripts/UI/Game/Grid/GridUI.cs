using System.Collections.Generic;
using System.Linq;
using Abilities;
using Grid;
using Managers;
using Turn;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UI.Game.Grid
{
    public class GridUI : DialogueComponent<GameDialogue>
    {
        [SerializeField] private LayerMask clickLayer;
        
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Required Components")]
        
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private LineRenderer selectedLineRenderer;
        
        private GridManager gridManager;
        private TurnManager turnManager;
        
        
        #region MonoBehaviour

        private void Start()
        {
            FillAll();
        }
        
        public void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame || Camera.main == null)
                return;
            
            Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(-Camera.main.transform.forward, transform.position);

            if (!plane.Raycast(worldRay, out float distance) || Physics.Raycast(worldRay, clickLayer))
                return;
            
            Vector2 worldPosition = worldRay.origin + worldRay.direction * distance;
            Vector2Int coordinate = gridManager.ConvertPositionToCoordinate(worldPosition);

            if (!gridManager.IsInBounds(coordinate))
                return;

            TryMove(coordinate);
        }
        
        #endregion
        
        
        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.abilitySelected.AddListener(OnAbilitySelected);
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
            dialogue.abilityRotated.AddListener(OnAbilityRotated);
            dialogue.abilityConfirmed.AddListener(OnAbilityConfirmed);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.abilitySelected.RemoveListener(OnAbilitySelected);
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityRotated.RemoveListener(OnAbilityRotated);
            dialogue.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
        }
        
        #endregion

        
        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            if (info.CurrentUnit.Unit is PlayerUnit)
                UpdateGrid();
            else
                FillAll();
        }

        private void OnAbilitySelected(Ability ability)
        {
            UpdateGrid();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            // TODO: Remove once UpdateGrid checks if an IUnit is moving...
            if (ability == null)
                return;
            
            UpdateGrid();
        }

        private void OnAbilityRotated(Vector2 direction)
        {
            // TODO: Remove once UpdateGrid checks if an IUnit is moving...
            if (dialogue.SelectedAbility == null)
                return;
            
            UpdateGrid();
        }

        private void OnAbilityConfirmed()
        {
            FillAll();
        }
        
        #endregion
        
        
        #region Drawing
        
        private void UpdateGrid()
        {
            // TODO: Add IUnit.IsMoving check whenever that's implemented...
            
            FillAll();
            
            // Draw the tiles in range of the selected ability or draw the tile in range of movement
            if (dialogue.SelectedAbility != null)
            {
                // TODO: Remove Where() when BasicShapeData.GetAffectedCoordinates() only returns in-bounds coordinates...
                Vector2Int[] coordinates = dialogue.SelectedAbility.Shape.
                    GetHighlightedCoordinates(turnManager.ActingUnit.Coordinate, dialogue.AbilityDirection).Where(vec => gridManager.IsInBounds(vec)).
                    ToArray();
                
                Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                
                //Outline(new GridSelection(coordinates, GridSelectionType.Valid));
            }
            else if (turnManager.ActingUnit.MovementPoints.Value > 0)
            {
                // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
                Vector2Int[] coordinates = turnManager.ActingUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
                Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                
                Vector2Int[] occupiedCoordinates = turnManager.ActingUnit.GetReachableOccupiedTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
                Fill(new GridSelection(occupiedCoordinates, GridSelectionType.Invalid));
            }
        }

        private void Outline(GridSelection selection)
        {
            Vector2Int[] tilePositions = new Vector2Int[4];
            
            // Get the max and min x and y values for the line renderer corners
            IOrderedEnumerable<Vector2Int> tileCoordinates = selection.Spaces.OrderByDescending(v => v.x);
            
            tilePositions[0] = tileCoordinates.First();
            tilePositions[2] = tileCoordinates.Last();
            
            tileCoordinates = selection.Spaces.OrderByDescending(v => v.y);
            
            tilePositions[1] = tileCoordinates.First();
            tilePositions[3] = tileCoordinates.Last();

            // Convert coordinates to Vector3 for the line renderer
            Vector3[] lineRendererPositions = new Vector3[4];

            for (int i = 0; i < tilePositions.Length; ++i)
            {
                lineRendererPositions[i] = new Vector3(tilePositions[i].x, tilePositions[i].y, 0);
            }
            
            // Set the line renderer positions
            selectedLineRenderer.SetPositions(lineRendererPositions);
        }
        
        private void Fill(GridSelection selection)
        {
            TileBase tile = GetTile(selection.Type);
            foreach (Vector2Int coordinate in selection.Spaces)
                tilemap.SetTile((Vector3Int) coordinate, tile);
        }
        
        private void FillAll(GridSelectionType type = GridSelectionType.Default)
        {
            BoundsInt b = gridManager.LevelBoundsInt;
            List<Vector2Int> coordinates = new List<Vector2Int>();
            
            for (int x = b.xMin; x <= b.xMax; x++)
            {
                for (int y = b.yMin; y <= b.yMax; y++)
                {
                    coordinates.Add(new Vector2Int(x, y));
                }
            }
            
            Fill(new GridSelection(coordinates.ToArray(), type));
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
        
        #endregion
        
        
        #region Movement
        
        private void TryMove(Vector2Int destination)
        {
            if (!(turnManager.ActingUnit is PlayerUnit playerUnit) || !turnManager.IsMovementPhase())
                return;

            // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
            List<Vector2Int> inRange = playerUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToList();
            
            if (!inRange.Contains(destination))
                return;
            
            dialogue.moveConfirmed.Invoke(new GameDialogue.MoveInfo(destination, dialogue.GetInfo(playerUnit)));
            FillAll();
        }
        
        #endregion
    }
}
