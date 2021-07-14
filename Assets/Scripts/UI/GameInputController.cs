using Grid;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    [AddComponentMenu("Playmakers/UI/Input Controller", 0)]
    internal class GameInputController : UIComponent<GameDialogue>
    {
        private GridManager gridManager;

        protected override void OnComponentAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                dialogue.abilityConfirmed.Invoke();

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                dialogue.unitDeselected.Invoke();

            if (Mouse.current.wasUpdatedThisFrame && Camera.main)
            {
                Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane plane = new Plane(-Camera.main.transform.forward, gridManager.LevelTilemap.transform.position.z);
            
                if (!plane.Raycast(worldRay, out float distance))
                    return;
            
                Vector2 worldPosition = worldRay.GetPoint(distance);
                dialogue.abilityRotated.Invoke(worldPosition);
            }
        }
    }
}
