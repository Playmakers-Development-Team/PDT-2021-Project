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
        
        #endregion

        
        #region MonoBehaviour Functions

        protected override void OnAwake()
        {
            manager.unitSpawned.AddListener(OnUnitSpawned);
        }

        protected void OnDisable()
        {
            manager.unitSpawned.RemoveListener(OnUnitSpawned);
        }
        
        #endregion
        
        
        #region Listener Methods

        private void OnUnitSpawned(IUnit unit)
        {
            // TODO: Instantiate a collider for the unit on spawn.
        }
        
        #endregion
    }
}
