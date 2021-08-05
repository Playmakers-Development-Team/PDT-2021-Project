using System;
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
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Required Components")]
        
        [SerializeField] private Tilemap tilemap;
        
        private GridManager gridManager;
        private TurnManager turnManager;

        private DisplayType displayType;

        private enum DisplayType
        {
            Default,
            Ability,
            Move
        }
        
        
        #region MonoBehaviour

        private void Start()
        {
            FillAll();
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
            dialogue.moveButtonPressed.AddListener(OnMoveButtonPressed);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.abilitySelected.RemoveListener(OnAbilitySelected);
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityRotated.RemoveListener(OnAbilityRotated);
            dialogue.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
            dialogue.moveButtonPressed.RemoveListener(OnMoveButtonPressed);
        }
        
        #endregion

        
        #region Listeners

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

            if (!gridManager.IsInBounds(coordinate))
                return;

            TryMove(coordinate);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            displayType = DisplayType.Default;
            
        }

        private void OnAbilitySelected(Ability ability)
        {
            displayType = DisplayType.Ability;
            UpdateGrid();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            displayType = DisplayType.Default;
            UpdateGrid();
        }

        private void OnAbilityRotated(Vector2 direction)
        {
            if (displayType == DisplayType.Ability)
                UpdateGrid();
        }

        private void OnAbilityConfirmed()
        {
            displayType = DisplayType.Default;
            UpdateGrid();
        }

        private void OnMoveButtonPressed(bool selected)
        {
            if (turnManager.ActingPlayerUnit == null)
                return;
            
            displayType = selected ? DisplayType.Move : DisplayType.Default;
            UpdateGrid();
        }
        
        #endregion
        
        
        #region Drawing
        
        private void UpdateGrid()
        {
            // TODO: Add IUnit.IsMoving check whenever that's implemented...
            
            FillAll();

            Vector2Int[] coordinates;
            switch (displayType)
            {
                case DisplayType.Ability when dialogue.SelectedAbility != null:
                    coordinates = dialogue.SelectedAbility.Shape.
                        GetHighlightedCoordinates(turnManager.ActingUnit.Coordinate, dialogue.AbilityDirection).
                        Where(vec => gridManager.IsInBounds(vec)).ToArray();
                    
                    Fill(new GridSelection(coordinates, GridSelectionType.Valid));
                    break;
                
                case DisplayType.Move when turnManager.ActingUnit.MovementPoints.Value > 0:
                    coordinates = turnManager.ActingUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();

                    Fill(new GridSelection(coordinates, GridSelectionType.Valid));
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
            if (displayType != DisplayType.Move)
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
