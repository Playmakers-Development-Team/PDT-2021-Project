using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    [AddComponentMenu("UI System/UI Controller", 0)]
    internal class Controller : Element
    {
        private void Update()
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
                manager.confirmedAbility.Invoke();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                manager.unitDeselected.Invoke();

            if (Mouse.current.wasUpdatedThisFrame && Camera.main)
            {
                Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane plane = new Plane(-Camera.main.transform.forward, transform.position);
            
                if (!plane.Raycast(worldRay, out float distance))
                    return;
            
                Vector2 worldPosition = worldRay.origin + worldRay.direction * distance;
                manager.rotatedAbility.Invoke(worldPosition);
            }
        }
    }
}
