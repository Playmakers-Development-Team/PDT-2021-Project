using System.Collections.Generic;
using Abilities;
using TMPro;
using UI.Core;
using Units;
using UnityEngine;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityList : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private List<AbilityCard> cards;
        
        [SerializeField] protected GameObject tooltipPanel;
        [SerializeField] protected TextMeshProUGUI tooltipDescription;
        
        
        #region UIComponent

        protected override void OnComponentStart()
        {
            tooltipPanel.SetActive(false);
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

        #endregion
        
        
        #region Listeners

        private void OnAbilityHoverEnter(AbilityCard card)
        {
            if (!cards.Contains(card))
                return;

            tooltipPanel.SetActive(true);
            tooltipDescription.text = card.Ability.Description + "\n\n";

            foreach (Keyword keyword in card.Ability.AllKeywords)
                tooltipDescription.text += $"{keyword.DisplayName}: {keyword.Description}\n\n";
        }

        private void OnAbilityHoverExit(AbilityCard card)
        {
            if (!cards.Contains(card))
                return;

            tooltipPanel.SetActive(false);
        }

        #endregion
        
        
        #region Drawing
        
        internal void Redraw(IUnit unit)
        {
            // STEP 1. Destroy AbilityCards in cards that no longer exist.
            for (int i = cards.Count - 1; i >= 0; i--)
            {
                if (unit.Abilities.Contains(cards[i].Ability))
                    continue;

                cards[i].Destroy();
                cards.RemoveAt(i);
            }
            
            // STEP 2. Instantiate new AbilityCards for new abilities.
            foreach (Ability ability in unit.Abilities)
            {
                if (cards.Find(card => card.Ability == ability))
                    continue;

                AbilityCard newCard = Instantiate(cardPrefab, transform).GetComponent<AbilityCard>();
                newCard.Assign(ability);

                cards.Add(newCard);
            }
        }
        
        #endregion
    }
}
