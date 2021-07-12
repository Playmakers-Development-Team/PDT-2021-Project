using System.Collections.Generic;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using Units;
using Units.Commands;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace UI
{
    // TODO: Move selectedAbility, abilityDirection to UIManager...
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
        private TurnManager turnManager;
        private CommandManager commandManager;
        private IUnit selectedUnit;
        private Ability selectedAbility;
        private Vector2 abilityDirection;


        private bool IsPlayerTurn => turnManager.ActingPlayerUnit != null &&
                                     selectedUnit != null &&
                                     turnManager.ActingUnit == selectedUnit;
        
        
        protected override void OnAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();

            manager.unitSelected.AddListener(OnUnitSelected);
            manager.unitDeselected.AddListener(OnUnitDeselected);
            
            manager.abilitySelected.AddListener(OnAbilitySelected);
            manager.abilityDeselected.AddListener(OnAbilityDeselected);
            manager.abilityRotated.AddListener(OnAbilityRotated);
            manager.abilityConfirmed.AddListener(OnAbilityConfirmed);
        }

        private void OnDisable()
        {
            manager.unitSelected.RemoveListener(OnUnitSelected);
            manager.unitDeselected.RemoveListener(OnUnitDeselected);
            
            manager.abilitySelected.RemoveListener(OnAbilitySelected);
            manager.abilityDeselected.RemoveListener(OnAbilityDeselected);
            manager.abilityRotated.RemoveListener(OnAbilityRotated);
            manager.abilityConfirmed.RemoveListener(OnAbilityConfirmed);
        }

        private void Start()
        {
            FillAll();
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

        private void UpdateGrid()
        {
            FillAll();
            
            if (!IsPlayerTurn)
                return;
            
            // Draw the tiles in range of the selected ability or draw the tile in range of movement
            if (selectedAbility != null)
            {
                // TODO: Remove Where() when BasicShapeData.GetAffectedCoordinates() only returns in-bounds coordinates...
                Vector2Int[] coordinates = selectedAbility.Shape.GetHighlightedCoordinates(selectedUnit.Coordinate, abilityDirection).
                    Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
                Fill(new GridSelection(coordinates, GridSelectionType.Valid));
            }
            else if (selectedUnit.MovementActionPoints.Value > 0)
            {
                // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
                // BUG: IndexOutOfRange when clicking unit while moving...
                Vector2Int[] coordinates = gridManager.GetAllReachableTiles(selectedUnit.Coordinate, selectedUnit.MovementActionPoints.Value).
                    Where(vec => gridManager.IsInBounds(vec)).ToArray();
                
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
            if (!IsPlayerTurn || selectedAbility != null)
                return;

            // TODO: Remove Where() when GetAffectedCoordinates() returns only in-bounds coordinates...
            List<Vector2Int> inRange = gridManager.GetAllReachableTiles(selectedUnit.Coordinate, selectedUnit.MovementActionPoints.Value).
                Where(vec => gridManager.IsInBounds(vec)).ToList();
            
            if (!inRange.Contains(destination))
                return;
            
            commandManager.ExecuteCommand(new StartMoveCommand(selectedUnit, destination));
            manager.unitDeselected.Invoke();
        }

        private void OnUnitSelected(IUnit unit)
        {
            OnUnitDeselected();
            selectedUnit = unit;
            UpdateGrid();
        }

        private void OnUnitDeselected()
        {
            selectedUnit = null;
            selectedAbility = null;
            UpdateGrid();
        }

        private void OnAbilitySelected(Ability ability)
        {
            selectedAbility = ability;
            UpdateGrid();
        }

        private void OnAbilityDeselected(Ability ability)
        {
            selectedAbility = null;
            UpdateGrid();
        }

        private void OnAbilityRotated(Vector2 direction)
        {
            abilityDirection = direction;
            UpdateGrid();
        }

        private void OnAbilityConfirmed()
        {
            if (!IsPlayerTurn || selectedAbility == null)
                return;
            
            commandManager.ExecuteCommand(new AbilityCommand(selectedUnit, abilityDirection, selectedAbility));
            commandManager.ExecuteCommand(new EndTurnCommand(selectedUnit));
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
