using System.Collections.Generic;
using Abilities;
using Abilities.Shapes;
using Audio;
using Commands;
using Grid;
using Managers;
using Turn;
using UI.Core;
using Units;
using Units.Virtual;
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

        private List<GameDialogue.ProjectedUnitInfo> lastProjected =
            new List<GameDialogue.ProjectedUnitInfo>();

        private bool CanUseAbility => turnManager.CanUseAbility;

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
                audioManager.ChangeMusicState("CombatState","InPauseMenu");
            }
            else
            {
                Destroy(pauseMenuInstance);
                pauseMenuInstance = null;
                audioManager.ChangeMusicState("CombatState","In_Combat");
                dialogue.Promote();

            }
        }
        
        

        #endregion

        private bool CanUseAbility => turnManager.CanUseAbility;

        #region MonoBehaviour
        
        private void Update()
        {
            if ((Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) && dialogue.DisplayMode == GameDialogue.Mode.Aiming)
            {
                if (turnManager.ActingPlayerUnit == null || dialogue.SelectedAbility == null || !CanUseAbility)
                    return;

                UseAbility(turnManager.ActingPlayerUnit, dialogue.SelectedAbility,
                    ShapeDirection.FromIsometric(dialogue.AbilityDirection));
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
                ShapeDirection shapeDirection = ShapeDirection.FromIsometric(direction);
                
                dialogue.abilityRotated.Invoke(direction);
                
                if (dialogue.SelectedAbility != null && CanUseAbility)
                    ProjectAbility(turnManager.ActingPlayerUnit, dialogue.SelectedAbility, shapeDirection);
            }
        }
        
        #endregion

        #region Abilities

        private void ProjectAbility(IUnit unit, Ability ability, ShapeDirection direction)
        {
            ClearLastProjection();
            var virtualUnits = unit.ProjectAbility(ability, direction);

            foreach (VirtualUnit virtualUnit in virtualUnits)
            {
                GameDialogue.ProjectedUnitInfo projectedUnitInfo =
                    new GameDialogue.ProjectedUnitInfo(virtualUnit, dialogue.GetInfo(virtualUnit.Unit));
                dialogue.unitApplyAbilityProjection.Invoke(projectedUnitInfo);
                lastProjected.Add(projectedUnitInfo);
            }
        }

        private void UseAbility(IUnit unit, Ability ability, ShapeDirection direction)
        {
            ClearLastProjection();
            unit.UseAbility(ability, direction);
        }

        private void ClearLastProjection()
        {
            foreach (GameDialogue.ProjectedUnitInfo projectedUnitInfo in lastProjected)
            {
                dialogue.unitCancelAbilityProjection.Invoke(projectedUnitInfo);
            }
            
            lastProjected.Clear();
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
