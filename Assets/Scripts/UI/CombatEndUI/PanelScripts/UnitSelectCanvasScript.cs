using System.Collections.Generic;
using UI.CombatEndUI.AbilityLoadout.Abilities;
using UI.CombatEndUI.AbilityLoadout.Unit;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI.PanelScripts
{
    public class UnitSelectCanvasScript : DialogueComponent<AbilityRewardDialogue>
    {
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private GameObject unitAbilityCardPrefab;
        
        public List<UnitCard> unitCards;
        [SerializeField] protected internal List<UnitAbilitiesCard> abilitiesCards;

        [SerializeField] private ScrollRect unitScrollView;
        [SerializeField] private ScrollRect abilityScrollView;

        [SerializeField] private Animator fadeOutTextAnim;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        protected override void OnComponentAwake()
        {
            unitScrollView.onValueChanged.AddListener(UpdateAbilityScroll);
        }

        #endregion

        #region Listeners
        
        public void OnAbilityButtonPress(AbilityButton abilityButton)
        {
            dialogue.activeAbilitiesCard.OnAbilityButtonPress(abilityButton);
        }
        
        #endregion

        #region Utility Functions

        public void ShowUnitSelectCanvas(LoadoutUnitInfo selectedUnit, float fadeOutTime)
        {
            SetActiveUnit(selectedUnit);
            
            DisableUnitButtons();
            
            FadeOutUnits(fadeOutTime);
            FadeOutText();
        }

        private void SetActiveUnit(LoadoutUnitInfo activeLoadoutUnitInfo)
        {
            // Get unit card spawn position
            Vector3 unitSpawnPosition = Vector3.zero;

            foreach (var unitCard in unitCards)
            {
                if (unitCard.loadoutUnitInfo == activeLoadoutUnitInfo)
                {
                    unitSpawnPosition = unitCard.transform.position;
                    break;
                }
            }

            // Instantiate and draw active unit
            dialogue.SetActiveUnitCard(Instantiate(unitCardPrefab,
                unitSpawnPosition,
                Quaternion.identity,
                transform)
                .GetComponent<UnitCard>());
            dialogue.activeUnitCard.GetComponentInChildren<Button>().interactable = false;
            dialogue.activeUnitCard.Redraw(activeLoadoutUnitInfo);
            
            
            // Get abilities card spawn position
            Vector3 abilitiesSpawnPosition = Vector3.zero;

            foreach (var abilitiesCard in abilitiesCards)
            {
                if (abilitiesCard.abilityInfos == activeLoadoutUnitInfo.AbilityInfo)
                {
                    abilitiesSpawnPosition = abilitiesCard.transform.position;
                    break;
                }
            }

            // Instantiate and draw abilities
            dialogue.activeAbilitiesCard = Instantiate(
                unitAbilityCardPrefab,
                abilitiesSpawnPosition,
                Quaternion.identity,
                transform)
                .GetComponent<UnitAbilitiesCard>();
            dialogue.activeAbilitiesCard.Redraw(activeLoadoutUnitInfo.AbilityInfo);
        }

        public void SlideActiveUnit()
        {
            dialogue.activeUnitCard.isSliding = true;
            dialogue.activeAbilitiesCard.isSliding = true;
        }
        
        public void RemoveSelectedAbility()
        {
            dialogue.activeAbilitiesCard.RemoveSelectedAbility(dialogue.activeUnitCard.loadoutUnitInfo);
            dialogue.UpdateCurrentAbilities();
        }
        
        private void UpdateAbilityScroll(Vector2 arg0)
        {
            abilityScrollView.horizontalNormalizedPosition = unitScrollView.horizontalNormalizedPosition;
        }

        public void EnableAbilityButtons()
        {
            dialogue.activeAbilitiesCard.EnableAbilityButtons();
        }

        private void FadeOutText()
        {
            fadeOutTextAnim.SetTrigger("Play");
        }
        
        // NOTE: Despite the name it doesn't fade in, it just makes it's alpha opaque
        // The FadeIn term is just used for consistency sake
        public void FadeInText()
        {
            fadeOutTextAnim.SetTrigger("PlayReverse");
        }

        private void DisableUnitButtons()
        {
            foreach (var unitCard in unitCards)
            {
                unitCard.DisableUnitButton();
            }
        }

        #endregion

        #region Drawing

        private void FadeOutUnits(float fadeOutTime)
        {
            foreach (var unit in unitCards)
                unit.FadeOut(fadeOutTime);

            foreach (var abilitiesCard in abilitiesCards)
                abilitiesCard.FadeOut(fadeOutTime);
        }

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
