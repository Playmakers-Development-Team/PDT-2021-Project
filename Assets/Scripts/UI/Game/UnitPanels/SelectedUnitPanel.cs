﻿using Commands;
using Managers;
using UI.Commands;
using UnityEngine;

namespace UI.Game.UnitPanels
{
    internal class SelectedUnitPanel : UnitPanel
    {
        [SerializeField] private Canvas canvas;


        #region UIComponent

        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.unitSelected.AddListener(OnUnitSelected);
            dialogue.unitDeselected.AddListener(OnUnitDeselected);
            dialogue.modeChanged.AddListener(OnModeChanged);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.unitSelected.RemoveListener(OnUnitSelected);
            dialogue.unitDeselected.RemoveListener(OnUnitDeselected);
            dialogue.modeChanged.RemoveListener(OnModeChanged);
        }

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            Hide();
        }

        #endregion


        #region Listeners

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            Redraw();
        }

        private void OnUnitSelected(GameDialogue.UnitInfo unit)
        {
            unitInfo = unit;

            Show();
            Redraw();

            ManagerLocator.Get<CommandManager>().ExecuteCommand(new UIUnitSelectedCommand(unit.Unit));
        }

        private void OnUnitDeselected()
        {
            unitInfo = null;
            Hide();
        }

        private void OnModeChanged(GameDialogue.Mode mode)
        {
            if (mode == GameDialogue.Mode.Default)
                return;

            Hide();
        }

        #endregion


        #region Drawing

        private void Hide() => canvas.enabled = false;

        private void Show() => canvas.enabled = true;

        #endregion
    }
}
