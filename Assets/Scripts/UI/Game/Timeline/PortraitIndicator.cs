using System;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using UI.Core;
using Units;
using Units.Enemies;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Timeline
{
    [RequireComponent(typeof(Image))]
    public class PortraitIndicator : DialogueComponent<GameDialogue>
    {
        [SerializeField] TimelinePortrait timelinePortrait;

        private TurnManager turnManager;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void Subscribe()
        {
            dialogue.turnManipulationStarted.AddListener(IndicatorOn);
            dialogue.turnManipulationEnded.AddListener(IndicatorOff);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnManipulationStarted.RemoveListener(IndicatorOn);
            dialogue.turnManipulationEnded.RemoveListener(IndicatorOff);
        }

        private void IndicatorOn(GameDialogue.UnitInfo unitInfo)
        {
            if (turnManager.ActingUnit == null)
                return;
            
            int actingUnitIndex = turnManager.FindTurnIndexFromCurrentQueue(turnManager.ActingUnit);
            int unitIndex = turnManager.FindTurnIndexFromCurrentQueue(timelinePortrait.UnitInfo.Unit);

            if (Mathf.Abs(unitIndex) - Mathf.Abs(actingUnitIndex) <= 1 && !timelinePortrait.IsSelected())
                transform.localScale = new Vector3(1, 1, 1);
        }

        private void IndicatorOff()
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
