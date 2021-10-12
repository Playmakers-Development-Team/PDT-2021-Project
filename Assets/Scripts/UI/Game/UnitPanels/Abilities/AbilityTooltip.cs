using System.Linq;
using Abilities;
using TMPro;
using UI.Core;
using UnityEngine;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityTooltip : DialogueComponent<GameDialogue>
    {
        [SerializeField] private AbilityList abilityList;
        [SerializeField] protected GameObject tooltipPanel;
        [SerializeField] protected TextMeshProUGUI tooltipDescription;

        protected override void OnComponentStart()
        {
            tooltipPanel.gameObject.SetActive(false);
        }

        protected override void Subscribe()
        {
            dialogue.abilityHoverEnter.AddListener(OnAbilityHoverEnter);
            dialogue.abilityHoverExit.AddListener(OnAbilityHoverExit);
        }

        protected override void Unsubscribe()
        {
            dialogue.abilityHoverEnter.RemoveListener(OnAbilityHoverEnter);
            dialogue.abilityHoverExit.RemoveListener(OnAbilityHoverExit);
        }

        #region Listeners

        private void OnAbilityHoverEnter(AbilityCard card)
        {
            if (!abilityList.Cards.Contains(card))
                return;

            tooltipPanel.SetActive(true);
            tooltipDescription.text = card.Ability.Description + "\n\n";

            foreach (Keyword keyword in card.Ability.AllKeywords)
                tooltipDescription.text += $"{keyword.DisplayName}: {keyword.Description}\n\n";
        }

        private void OnAbilityHoverExit(AbilityCard card)
        {
            if (!abilityList.Cards.Contains(card))
                return;

            tooltipPanel.SetActive(false);
        }
        
        #endregion
    }
}
