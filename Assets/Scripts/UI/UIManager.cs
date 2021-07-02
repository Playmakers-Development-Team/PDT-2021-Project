using System;
using System.Linq;
using Abilities;
using Commands;
using Managers;
using Units;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class UIManager : Manager
    {
        public readonly Event<IUnit> selectedUnit = new Event<IUnit>();
        public readonly Event deselectedUnit = new Event();

        public readonly Event<GridSelection> gridSpacesSelected = new Event<GridSelection>();
        public readonly Event gridSpacesDeselected = new Event();
        public readonly Event<Vector2Int> gridClicked = new Event<Vector2Int>();
        
        public readonly Event turnChanged = new Event();

        public readonly Event<IUnit> unitChanged = new Event<IUnit>();

        public readonly Event<IUnit> unitSpawned = new Event<IUnit>();

        public readonly Event<Ability> selectedAbility = new Event<Ability>();
        public readonly Event deselectedAbility = new Event();
        public readonly Event rotatedAbility = new Event();
        public readonly Event confirmedAbility = new Event();

        private TurnManager turnManager;
        private GridManager gridManager;
        private CommandManager commandManager;

        private Ability currentAbility;
        private IUnit currentUnit;

        private Vector2 abilityDirection = Vector2.up;


        private bool IsPlayerTurn => turnManager.ActingUnit != null && turnManager.ActingPlayerUnit == (PlayerUnit) currentUnit;

        private bool IsAbilitySelected => currentAbility != null;


        public override void ManagerStart()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            
            selectedUnit.AddListener(OnUnitSelected);
            deselectedUnit.AddListener(OnUnitDeselected);

            selectedAbility.AddListener(OnSelectedAbility);
            rotatedAbility.AddListener(OnRotatedAbility);
            deselectedAbility.AddListener(OnDeselectedAbility);
            confirmedAbility.AddListener(OnConfirmedAbility);

            turnChanged.AddListener(OnTurnChanged);

            gridClicked.AddListener(OnGridClicked);

            commandManager.ListenCommand((StartTurnCommand a) =>
            {
                turnChanged.Invoke();
            });
            
            commandManager.ListenCommand((TurnQueueCreatedCommand c) =>
            {
                turnChanged.Invoke();
            });
        }

        
        #region Abilities

        private void OnSelectedAbility(Ability ability)
        {
            if (IsPlayerTurn)
                return;
            
            currentAbility = ability;
            UpdateGrid();
        }

        private void OnRotatedAbility()
        {
            if (IsPlayerTurn && IsAbilitySelected)
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
            if (currentAbility == null)
                return;
            
            // TODO: Execute ability here...
            Debug.Log($"{currentAbility.name} confirmed!");
        }
        
        #endregion
        
        
        #region Grid
        
        private void UpdateGrid()
        {
            // Clear current grid
            gridSpacesDeselected.Invoke();
            
            // Highlight ability squares
            if (currentAbility != null && currentUnit != null)
            {
                Vector2Int[] coordinates = currentAbility.Shape.GetHighlightedCoordinates(currentUnit.Coordinate, abilityDirection).ToArray();
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Valid));
            }
            
            // Highlight movement squares
            if (currentAbility == null && currentUnit != null)
            {
                Vector2Int[] coordinates = gridManager.GetAllReachableTiles(currentUnit.Coordinate, (int) currentUnit.MovementActionPoints.Value).
                    ToArray();
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Valid));
            }

            // Highlight square under active IUnit
            if (currentUnit != null)
            {
                Vector2Int[] coordinates = {currentUnit.Coordinate};
                gridSpacesSelected.Invoke(new GridSelection(coordinates, GridSelectionType.Selected));
            }
        }

        private void OnGridClicked(Vector2Int destination)
        {
            TryMove(destination);
        }

        private void TryMove(Vector2Int destination)
        {
            if (IsPlayerTurn && !IsAbilitySelected)
                return;
            
            // TODO: Move unit...
            Debug.Log($"Moving unit to {destination}!");
        }
        
        #endregion
        
        
        #region Unit Selection

        private void OnUnitSelected(IUnit unit) => currentUnit = unit;

        private void OnUnitDeselected()
        {
            currentUnit = null;
            deselectedAbility.Invoke();
        }

        #endregion
        
        
        #region Timeline

        private void OnTurnChanged()
        {
            UpdateGrid();
        }
        
        #endregion
    }
}