using UI.CombatEndUI.AbilityLoadout;
using UI.Core;
using UnityEngine;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class ReturnButton : DialogueComponent<AbilityRewardDialogue>
    {
        public void OnPressed()
        {
            dialogue.showUnitSelectPanel.Invoke();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
    }
}
