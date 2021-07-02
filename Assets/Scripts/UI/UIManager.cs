using System;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using Units;
using Units.Commands;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class UIManager : Manager
    {
        // Unit
        public readonly Event<IUnit> unitSelected = new Event<IUnit>();
        public readonly Event unitDeselected = new Event();

        public readonly Event<IUnit> unitChanged = new Event<IUnit>();
        
        // Abilities
        public readonly Event<Ability> selectedAbility = new Event<Ability>();
        public readonly Event deselectedAbility = new Event();
        
        public readonly Event rotatedAbility = new Event();
        
        public readonly Event confirmedAbility = new Event();

        // Grid
        public readonly Event<GridSelection> gridSpacesSelected = new Event<GridSelection>();
        public readonly Event gridSpacesDeselected = new Event();
        
        public readonly Event<Vector2Int> gridClicked = new Event<Vector2Int>();
        
        // Turn
        public readonly Event turnChanged = new Event();
        

        private TurnManager turnManager;
        private GridManager gridManager;
        private CommandManager commandManager;
        
        private Ability currentAbility;
        private IUnit selectedUnit;

        private Vector2 abilityDirection = Vector2.up;


        private bool IsPlayerTurn => selectedUnit is PlayerUnit asPlayer && asPlayer == turnManager.ActingPlayerUnit;
        private bool IsAbilitySelected => currentAbility != null;


        public override void ManagerStart()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            
            unitSelected.AddListener(OnUnitSelected);
            unitDeselected.AddListener(OnUnitDeselected);

            selectedAbility.AddListener(OnSelectedAbility);
            rotatedAbility.AddListener(OnRotatedAbility);
            deselectedAbility.AddListener(OnDeselectedAbility);
            confirmedAbility.AddListener(OnConfirmedAbility);

            turnChanged.AddListener(OnTurnChanged);

            gridClicked.AddListener(OnGridClicked);

            commandManager.ListenCommand((StartTurnCommand c) => turnChanged.Invoke());
            commandManager.ListenCommand((EndTurnCommand c) => turnChanged.Invoke());
            commandManager.ListenCommand((TurnQueueCreatedCommand c) => turnChanged.Invoke());
        }

        
        #region Abilities

        private void OnSelectedAbility(Ability ability)
        {
            if (!IsPlayerTurn)
                return;
            
            currentAbility = ability;
            UpdateGrid();
        }

        private void OnRotatedAbility()
        {
            if (!IsPlayerTurn || !IsAbilitySelected)
                return;

            abilityDirection = Quaternion.AngleAxis(90f, Vector3.back) * abilityDirection;
            UpdateGrid();
        }

        private void OnDeselectedAbility()
        {
            currentAbility = null;
            UpdateGrid();
        }

        private void OnConfirmedAbility()
        {
            if (!IsPlayerTurn || !IsAbilitySelected)
                return;
            
            Debug.Log($"Executing ability command: {currentAbility.name}");
            commandManager.ExecuteCommand(new AbilityCommand(selectedUnit, abilityDirection, currentAbility));
            commandManager.ExecuteCommand(new EndTurnCommand(selectedUnit));
        }
        
        #endregion
        
        
        #region Grid
        
        private void UpdateGrid()
        {
            // Clear current grid
            gridSpacesDeselected.Invoke();
            
            // Highlight ability squares
            if (IsPlayerTurn && IsAbilitySelected)
            {
                Vector2Int[] coordinates = currentAbility.Shape.GetHighlightedCoordinates(selectedUnit.Coordinate, abilityDirection).ToArray();
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Valid));
            }
            
            // Highlight movement squares
            if (IsPlayerTurn && !IsAbilitySelected)
            {
                Vector2Int[] coordinates = gridManager.GetAllReachableTiles(selectedUnit.Coordinate, (int) selectedUnit.MovementActionPoints.Value).
                    ToArray();
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Valid));
            }

            // Highlight square under active IUnit
            if (IsPlayerTurn)
            {
                Vector2Int[] coordinates = {selectedUnit.Coordinate};
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Selected));
            }
        }

        private void OnGridClicked(Vector2Int destination)
        {
            TryMove(destination);
        }

        private void TryMove(Vector2Int destination)
        {
            if (!IsPlayerTurn || IsAbilitySelected)
                return;
            
            // TODO: Move unit...
            Debug.Log($"Moving unit to {destination}!");
        }
        
        #endregion
        
        
        #region Unit Selection

        private void OnUnitSelected(IUnit unit)
        {
            selectedUnit = unit;
            UpdateGrid();
        }

        private void OnUnitDeselected()
        {
            selectedUnit = null;
            deselectedAbility.Invoke();
        }

        #endregion
        
        
        #region Timeline

        private void OnTurnChanged()
        {
            unitDeselected.Invoke();
            UpdateGrid();
        }
        
        #endregion
    }
}