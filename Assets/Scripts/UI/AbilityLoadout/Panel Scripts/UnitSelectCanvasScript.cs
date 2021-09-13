using System.Collections.Generic;
using UI.AbilityLoadout.Abilities;
using UI.AbilityLoadout.Unit;
using UI.Core;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Panel_Scripts
{
    public class UnitSelectCanvasScript : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private GameObject unitAbilityCardPrefab;
        
        [SerializeField] private List<UnitCard> unitCards;
        [SerializeField] private List<UnitAbilitiesCard> abilitiesCards;

        [SerializeField] private ScrollRect unitScrollView;
        [SerializeField] private ScrollRect abilityScrollView;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        protected override void OnComponentAwake()
        {
            unitScrollView.onValueChanged.AddListener(UpdateAbilityScroll);
        }

        #endregion

        #region Utility Functions

        private void UpdateAbilityScroll(Vector2 arg0)
        {
            abilityScrollView.horizontalNormalizedPosition = unitScrollView.horizontalNormalizedPosition;
        }

        public void EnableAbilityButtons(AbilityLoadoutDialogue.UnitInfo unitInfo)
        {
            foreach (var abilityCard in abilitiesCards)
            {
                if (unitInfo.AbilityInfo == abilityCard.abilityInfos)
                    abilityCard.EnableAbilityButtons();
            }
            
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<AbilityLoadoutDialogue.UnitInfo> units)
        {
            // STEP 1. Destroy UnitCards and UnitAbilityCards
            for (int i = unitCards.Count - 1; i >= 0; i--)
            {
                Destroy(unitCards[i].gameObject);
                Destroy(abilitiesCards[i].gameObject);
                    
                unitCards.RemoveAt(i);
                abilitiesCards.RemoveAt(i);
            }

            // STEP 2. Instantiate new UnitCards and UnitAbilityCards for new units.
            foreach (AbilityLoadoutDialogue.UnitInfo unit in units)
            {
                UnitCard newCard = Instantiate(unitCardPrefab, unitScrollView.content).GetComponent<UnitCard>();
                newCard.Redraw(unit);

                unitCards.Add(newCard);
                
                UnitAbilitiesCard abilityCard = Instantiate(unitAbilityCardPrefab, abilityScrollView.content).GetComponent<UnitAbilitiesCard>();
                abilityCard.Redraw(unit.AbilityInfo);

                abilitiesCards.Add(abilityCard);
            }
        }
        
        #endregion
    }
}
