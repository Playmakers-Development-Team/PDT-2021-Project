using System.Collections.Generic;
using Managers;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.UnitPanels.Abilities
{
    public class AbilityLoadoutCharacterList : DialogueComponent<GameDialogue>
    {
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private List<CharacterCard> cards;

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
        
        internal void Redraw()
        {
            if (playerManager.Units == null)
                return;
            
            Debug.Log("There are "+playerManager.Units.Count+" player units");
        }
        
        #endregion
    }
}
