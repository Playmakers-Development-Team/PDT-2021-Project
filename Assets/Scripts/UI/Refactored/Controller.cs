using System;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Refactored
{
    [AddComponentMenu("UI System/UI Controller", 0)]
    internal class Controller : Element
    {
        // TODO: Remove when no longer required for testing.
        #region TESTING - REMOVE WHEN COMPLETE

        [ContextMenu("Next Turn")]
        private void NextTurn()
        {
            manager.turnChanged.Invoke();
        }
        
        #endregion

        private void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                manager.rotatedAbility.Invoke();

            if (Keyboard.current.enterKey.wasPressedThisFrame)
                manager.confirmedAbility.Invoke();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                manager.deselectedUnit.Invoke();
        }
    }
}
