using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    [AddComponentMenu("UI System/UI Controller", 0)]
    internal class Controller : Element
    {
        private void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                manager.rotatedAbility.Invoke();

            if (Keyboard.current.enterKey.wasPressedThisFrame)
                manager.confirmedAbility.Invoke();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                manager.unitDeselected.Invoke();
        }
    }
}
