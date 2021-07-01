using System;
using Units;
using UnityEngine;

namespace UI.Refactored
{
    [AddComponentMenu("UI System/UI Controller", 0)]
    internal class Controller : Element
    {
        // TODO: Remove when no longer required for testing.
        #region TESTING - REMOVE WHEN COMPLETE
        
        [SerializeField] private PlayerUnit testUnit;

        [ContextMenu("Select Test Unit")]
        private void SelectTestUnit()
        {
            if (testUnit == null)
                return;
            
            manager.selectedUnit.Invoke(testUnit);
        }

        [ContextMenu("Deselect Test Unit")]
        private void DeselectTestUnit()
        {
            manager.deselectedUnit.Invoke();
        }

        [ContextMenu("Change Test Unit")]
        private void ChangeTestUnit()
        {
            testUnit.gameObject.name = "New Name";
            testUnit.TakeDamage(5);
            testUnit.TakeAttack(2);
            testUnit.TakeDefence(1);

            manager.unitChanged.Invoke(testUnit);
        }

        [ContextMenu("Select Grid")]
        private void SelectGrid()
        {
            GridSelection selection = new GridSelection(
                new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1)
                },
                GridSelectionType.Valid
                );

            manager.gridSpacesSelected.Invoke(selection);
        }

        [ContextMenu("Deselect Grid")]
        private void DeselectGrid()
        {
            manager.gridSpacesDeselected.Invoke();
        }
        
        #endregion
    }
}
