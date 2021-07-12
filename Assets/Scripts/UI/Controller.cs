using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    [AddComponentMenu("UI System/UI Controller", 0)]
    internal class Controller : Element
    {
        private GridManager gridManager;

        protected override void OnAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
        }

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                manager.abilityConfirmed.Invoke();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                manager.unitDeselected.Invoke();

            if (Mouse.current.wasUpdatedThisFrame && Camera.main)
            {
                Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane plane = new Plane(-Camera.main.transform.forward, gridManager.LevelTilemap.transform.position.z);
            
                if (!plane.Raycast(worldRay, out float distance))
                    return;
            
                Vector2 worldPosition = worldRay.GetPoint(distance);
                manager.abilityRotated.Invoke(worldPosition);
            }
        }
    }
}
