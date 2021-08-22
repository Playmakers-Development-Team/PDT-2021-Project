using System.Collections.Generic;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout.Unit
{
    public class AbilityLoadoutUnitList : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private List<UnitCard> cards;

        private ScrollRect scrollView;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out scrollView);
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<AbilityLoadoutDialogue.UnitInfo> units)
        {
            // Instantiate new UnitCards
            foreach (AbilityLoadoutDialogue.UnitInfo unit in units)
            {
                UnitCard newCard = Instantiate(unitCardPrefab, scrollView.content).GetComponent<UnitCard>();
                newCard.Redraw(unit);

                cards.Add(newCard);
            }
        }
        
        #endregion
    }
}
