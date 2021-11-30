using System.Collections.Generic;
using Abilities;
using Abilities.Shapes;
using Audio;
using Game;
using Grid;
using Managers;
using Turn;
using UI.Core;
using UI.Input;
using Units;
using Units.Virtual;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace UI.Game
{
    internal class GameInputController : DialogueComponent<GameDialogue>
    {
        private GameManager gameManager;
        private GridManager gridManager;
        private TurnManager turnManager;
        private AudioManager audioManager;
        private PlayerControls playerControls;

        private List<GameDialogue.ProjectedUnitInfo> lastProjected =
            new List<GameDialogue.ProjectedUnitInfo>();

        private bool CanUseAbility => turnManager.CanUseAbility;

        [SerializeField] private GameObject pauseMenu;

        private bool paused;

        #region DelegateFunctions

        private void PauseGame(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            if (gameManager.IsPaused)
            {
                manager.Pop();

                gameManager.Resume();
            }
            else
            {
                audioManager.ChangeMusicState("CombatState", "InPauseMenu");
            
                Instantiate(pauseMenu, dialogue.transform.parent);
                
                gameManager.Pause();
            }

            paused = !paused;
        }

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if ((Mouse.current.rightButton.wasPressedThisFrame ||
                 Keyboard.current.spaceKey.wasPressedThisFrame) &&
                dialogue.DisplayMode == GameDialogue.Mode.Aiming)
            {
                if (turnManager.ActingPlayerUnit == null || dialogue.SelectedAbility == null ||
                    !CanUseAbility)
                    return;

                UseAbility(turnManager.ActingPlayerUnit, dialogue.SelectedAbility,
                    ShapeDirection.FromIsometric(dialogue.AbilityDirection));
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                dialogue.unitDeselected.Invoke();

            if (turnManager.ActingPlayerUnit != null && Mouse.current.wasUpdatedThisFrame &&
                Camera.main)
            {
                Ray worldRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane plane = new Plane(-Camera.main.transform.forward,
                    gridManager.LevelTilemap.transform.position.z);

                if (!plane.Raycast(worldRay, out float distance))
                    return;

                Vector2 worldPosition = worldRay.GetPoint(distance);
                Vector2 direction =
                    (worldPosition -
                     gridManager.ConvertCoordinateToPosition(
                         turnManager.ActingPlayerUnit.Coordinate)).normalized;
                ShapeDirection shapeDirection = ShapeDirection.FromIsometric(direction);

                dialogue.abilityRotated.Invoke(direction);

                if (dialogue.SelectedAbility != null && CanUseAbility)
                    ProjectAbility(turnManager.ActingPlayerUnit, dialogue.SelectedAbility,
                        shapeDirection);
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
                    new GameDialogue.ProjectedUnitInfo(virtualUnit,
                        dialogue.GetInfo(virtualUnit.Unit));
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
            gameManager = ManagerLocator.Get<GameManager>();
            audioManager = ManagerLocator.Get<AudioManager>();
            gridManager = ManagerLocator.Get<GridManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
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
