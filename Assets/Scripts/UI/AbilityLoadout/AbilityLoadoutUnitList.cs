using System.Collections.Generic;
using Managers;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AbilityLoadout
{
    public class AbilityLoadoutUnitList : DialogueComponent<AbilityLoadoutDialogue>
    {
        [SerializeField] private GameObject unitCardPrefab;
        [SerializeField] private List<UnitCard> cards;

        private ScrollRect scrollView;
        
        private PlayerManager playerManager;

        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}
        
        protected override void OnComponentAwake()
        {
            TryGetComponent(out scrollView);
            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        #endregion

        #region Drawing
        
        internal void Redraw(List<AbilityLoadoutDialogue.UnitInfo> units)
        {
            // Instantiate new UnitCards
            foreach (AbilityLoadoutDialogue.UnitInfo unit in units)
            {
                UnitCard newCard = Instantiate(unitCardPrefab, scrollView.content).GetComponent<UnitCard>();
                newCard.Assign(unit);

                cards.Add(newCard);
            }
        }
        
        #endregion
    }
}
