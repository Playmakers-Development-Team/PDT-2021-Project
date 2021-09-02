using Abilities.Commands;
using Commands;
using Grid;
using Managers;
using Turn;
using UI.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Game
{
    internal class GameInputController : DialogueComponent<GameDialogue>
    {
        private GridManager gridManager;
        private TurnManager turnManager;
        private CommandManager commandManager;


        #region MonoBehaviour
        
        private void Update()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (turnManager.ActingPlayerUnit == null || dialogue.SelectedAbility == null || !turnManager.CanUseAbility)
                    return;

                commandManager.ExecuteCommand(new AbilityCommand(turnManager.ActingPlayerUnit,
                    dialogue.AbilityDirection, dialogue.SelectedAbility));
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
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
            commandManager = ManagerLocator.Get<CommandManager>();
        }
        
        #endregion
    }
}
