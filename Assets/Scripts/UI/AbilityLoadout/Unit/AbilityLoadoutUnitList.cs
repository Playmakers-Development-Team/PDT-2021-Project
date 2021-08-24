using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            // STEP 1. Destroy UnitCards in cards that no longer exist.
            for (int i = cards.Count - 1; i >= 0; i--)
            {
                if (units.Contains(cards[i].unitInfo))
                {
                    Destroy(cards[i].gameObject);
                    cards.RemoveAt(i);
                }
            }

            // STEP 2. Instantiate new UnitCards for new units.
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
