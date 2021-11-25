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
        protected override void Subscribe()
        {
            dialogue.turnManipulationStarted.AddListener(indicatorOn);
            dialogue.turnManipulationEnded.AddListener(indicatorOff);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnManipulationStarted.RemoveListener(indicatorOn);
            dialogue.turnManipulationEnded.RemoveListener(indicatorOff);
        }

        private void indicatorOn(GameDialogue.UnitInfo unitInfo)
        {
            if(!timelinePortrait.isSelected())
                transform.localScale = new Vector3(1, 1, 1);
        }

        private void indicatorOff()
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
