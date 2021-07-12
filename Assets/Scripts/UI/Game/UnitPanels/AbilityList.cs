using System.Collections.Generic;
using Abilities;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityList : Element
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private List<AbilityCard> cards;

        private ScrollRect scrollView;
        

        protected override void OnAwake()
        {
            TryGetComponent(out scrollView);
        }

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

                AbilityCard newCard = Instantiate(cardPrefab, scrollView.content).GetComponent<AbilityCard>();
                newCard.Assign(ability);

                cards.Add(newCard);
            }
        }
    }
}
