using System.Collections.Generic;
using UI.CombatEndUI.AbilityLoadout;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.CombatEndUI.AbilityLoadout.Unit;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.PanelScripts
{
    public class UnitSelectCanvasScript : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private GameObject unitAbilityCardPrefab;
        
        public List<UnitCard> unitCards;
        [SerializeField] protected internal List<UnitAbilitiesCard> abilitiesCards;

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

        #region Listeners

        // Assumption is that the ability card will always be the first in the list
        // since this function will only be called when there is only 1 unit on screen
        // (ability buttons shouldn't be interactable otherwise)
        public void OnAbilityButtonPress(AbilityButton abilityButton)
        {
            abilitiesCards[0].OnAbilityButtonPress(abilityButton);
        }
        
        #endregion

        #region Utility Functions

        // Same assumption stated previously for OnAbilityButtonPress applies here
        public void RemoveSelectedAbility()
        {
            abilitiesCards[0].RemoveSelectedAbility(unitCards[0].loadoutUnitInfo.Unit);
        }
        
        private void UpdateAbilityScroll(Vector2 arg0)
        {
            abilityScrollView.horizontalNormalizedPosition = unitScrollView.horizontalNormalizedPosition;
        }

        public void EnableAbilityButtons(LoadoutUnitInfo loadoutUnitInfo)
        {
            foreach (var abilityCard in abilitiesCards)
            {
                if (loadoutUnitInfo.AbilityInfo == abilityCard.abilityInfos)
                    abilityCard.EnableAbilityButtons();
            }
            
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<LoadoutUnitInfo> units)
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
            foreach (LoadoutUnitInfo unit in units)
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
