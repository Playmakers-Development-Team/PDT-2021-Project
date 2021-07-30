using Grid;
using Managers;
using Turn;
using UI.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace UI.Game
{
    internal class GameInputController : DialogueComponent<GameDialogue>
    {
        private GridManager gridManager;
        private TurnManager turnManager;


        #region MonoBehaviour
        
        private void Update()
        {
            if (Mouse.current.rightButton.isPressed)
                dialogue.abilityConfirmed.Invoke();

            if (Keyboard.current.escapeKey.isPressed)
                dialogue.unitDeselected.Invoke();

            if (turnManager.ActingPlayerUnit != null &&  Mouse.current.wasUpdatedThisFrame && Camera.main)
            {
                Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane plane = new Plane(-Camera.main.transform.forward, gridManager.LevelTilemap.transform.position.z);
                
                if (!plane.Raycast(worldRay, out float distance))
                    return;
            
                Vector2 worldPosition = worldRay.GetPoint(distance);
                Vector2 direction = (worldPosition - gridManager.ConvertCoordinateToPosition(turnManager.ActingPlayerUnit.Coordinate)).normalized;
                
                dialogue.abilityRotated.Invoke(direction);
            }
        }
        
        #endregion
        
        
        #region UIComponent

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }
        
        #endregion
    }
}
