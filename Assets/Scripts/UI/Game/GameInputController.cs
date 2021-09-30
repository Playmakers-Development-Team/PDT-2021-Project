using Abilities.Commands;
using Audio;
using Commands;
using Grid;
using Managers;
using Turn;
using UI.Core;
using UI.Input;
using UI.PauseScreen;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Game
{
    internal class GameInputController : DialogueComponent<GameDialogue>
    {
        private GridManager gridManager;
        private TurnManager turnManager;
        private AudioManager audioManager;
        private CommandManager commandManager;
        private PlayerControls playerControls;
        private GameObject pauseMenuInstance;

        [SerializeField] private Transform parent;
        

        [SerializeField] private GameObject PauseMenu;

        #region DelegateFunctions

        private void PauseGame(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            if (pauseMenuInstance == null)
            {
                pauseMenuInstance = Instantiate(PauseMenu,parent);
                pauseMenuInstance.GetComponent<PauseScreenDialogue>().GameDialgoue = dialogue;
                audioManager.UpdateMusic("CombatState","InPauseMenu");
            }
            else
            {
                Destroy(pauseMenuInstance);
                pauseMenuInstance = null;
                audioManager.UpdateMusic("CombatState","In_Combat");
                dialogue.Promote();

            }
        }
        
        

        #endregion

        #region MonoBehaviour
        
        private void Update()
        {
            if ((Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && dialogue.DisplayMode == GameDialogue.Mode.Aiming)
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
            audioManager = ManagerLocator.Get<AudioManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            playerControls = new PlayerControls();
            playerControls.UI.Pause.performed += PauseGame;

        }

        protected override void OnComponentEnabled()
        {
            playerControls.Enable();
        }

        protected override void OnComponentDisabled()
        {
            playerControls.Disable();
        }

        #endregion
    }
}
