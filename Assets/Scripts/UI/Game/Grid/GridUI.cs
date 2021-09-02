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
            dialogue.abilitySelected.AddListener(OnAbilitySelected);
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
            dialogue.abilityRotated.AddListener(OnAbilityRotated);
            dialogue.abilityConfirmed.AddListener(OnAbilityConfirmed);
            dialogue.modeChanged.AddListener(OnModeChanged);
        }

        protected override void Unsubscribe()
        {
            dialogue.abilitySelected.RemoveListener(OnAbilitySelected);
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityRotated.RemoveListener(OnAbilityRotated);
            dialogue.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
            dialogue.modeChanged.RemoveListener(OnModeChanged);
        }
        
        #endregion

        
        #region Listeners

        private void OnAbilitySelected(Ability ability)
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Aiming);
            UpdateGrid();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Default);
            UpdateGrid();
        }

        private void OnAbilityRotated(Vector2 direction)
        {
            if (dialogue.DisplayMode == GameDialogue.Mode.Aiming)
                UpdateGrid();
        }

        private void OnAbilityConfirmed()
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Default);
            UpdateGrid();
        }

        private void OnModeChanged(GameDialogue.Mode mode)
        {
            UpdateGrid();
        }
        
        #endregion
        
        
        #region Drawing
        
        private void UpdateGrid()
        {
            // TODO: Add IUnit.IsMoving check whenever that's implemented...
            
            FillAll();

            Vector2Int[] coordinates;
            switch (dialogue.DisplayMode)
            {
                case GameDialogue.Mode.Aiming when dialogue.SelectedAbility != null:
                    coordinates = dialogue.SelectedAbility.Shape.
                        GetHighlightedCoordinates(turnManager.ActingUnit.Coordinate, dialogue.AbilityDirection).
                        Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    
                    Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                    break;
                
                case GameDialogue.Mode.Moving when turnManager.ActingUnit.MovementPoints.Value > 0:
                    coordinates = turnManager.ActingUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();

                    Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                    
                    Vector2Int[] occupiedCoordinates = turnManager.ActingUnit.GetReachableOccupiedTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
                    Fill(new GridSelection(occupiedCoordinates, GridSelectionType.Invalid));
                    
                    break;
            }
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
        
        // TODO: This would probably be better being in InputController...
        private void TryMove(Vector2Int destination)
        {
            if (dialogue.DisplayMode != GameDialogue.Mode.Moving)
                return;

            PlayerUnit playerUnit = turnManager.ActingPlayerUnit;
            
            // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
            List<Vector2Int> inRange = playerUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToList();
            
            if (!inRange.Contains(destination))
                return;
            
            dialogue.moveConfirmed.Invoke(new GameDialogue.MoveInfo(destination, dialogue.GetInfo(playerUnit)));
            dialogue.buttonSelected.Invoke();
            
            FillAll();
        }
        
        #endregion
    }
}
