﻿using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Grid;
using Managers;
using Turn;
using Units.Commands;
using Units.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UI
{
    public class GridUI : UIComponent<GameDialogue>
    {
        [Header("Tile types")]
        
        [SerializeField] private TileBase defaultTile;
        [SerializeField] private TileBase validTile;
        [SerializeField] private TileBase invalidTile;
        [SerializeField] private TileBase selectedTile;
        
        [Header("Required Components")]
        
        [SerializeField] private Tilemap tilemap;
        
        private GridManager gridManager;
        private CommandManager commandManager;
        private TurnManager turnManager;
        
        
        #region UIComponent
        
        protected override void OnComponentAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.abilitySelected.AddListener(OnAbilitySelected);
            dialogue.abilityDeselected.AddListener(OnAbilityDeselected);
            dialogue.abilityRotated.AddListener(OnAbilityRotated);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.abilitySelected.RemoveListener(OnAbilitySelected);
            dialogue.abilityDeselected.RemoveListener(OnAbilityDeselected);
            dialogue.abilityRotated.RemoveListener(OnAbilityRotated);
        }
        
        #endregion
        
        
        #region MonoBehaviour

        private void Start()
        {
            FillAll();
        }
        
        #endregion

        
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
            }
            else if (turnManager.ActingUnit.MovementActionPoints.Value > 0)
            {
                // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
                Vector2Int[] coordinates = turnManager.ActingUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
                Fill(new GridSelection(coordinates, GridSelectionType.Valid));
            }
        }

        private void Fill(GridSelection selection)
        {
            TileBase tile = GetTile(selection.Type);
            foreach (Vector2Int coordinate in selection.Spaces)
                tilemap.SetTile((Vector3Int) coordinate, tile);
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

            if (!gridManager.IsInBounds(coordinate))
                return;

            TryMove(coordinate);
        }
        
        private void TryMove(Vector2Int destination)
        {
            if (!(turnManager.ActingUnit is PlayerUnit playerUnit))
                return;

            // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
            List<Vector2Int> inRange = playerUnit.GetAllReachableTiles().Where(vec => gridManager.IsInBounds(vec)).ToList();
            
            if (!inRange.Contains(destination))
                return;
            
            FillAll();
            commandManager.ExecuteCommand(new StartMoveCommand(playerUnit, destination));
        }

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
    }
}
