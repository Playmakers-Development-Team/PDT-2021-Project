using System;
using Commands;
using Game.Commands;
using Managers;
using Turn.Commands;
using UI.Core;
using UnityEngine;
using Event = UI.Core.Event;

namespace UI.AbilityLoadout
{
    public class AbilityLoadoutDialogue : Dialogue
    {
        internal readonly Event<AbilityLoadoutPanelType> panelSwap = new Event<AbilityLoadoutPanelType>();
        internal readonly Event encounterStart = new Event();
        internal readonly Event encounterWon = new Event();
        
        private CommandManager commandManager;

        [SerializeField] private Canvas characterSelectPanel;
        [SerializeField] private Canvas abilitySelectPanel;

        #region Monobehaviour Events

        internal override void OnAwake()
        {
            // Assign Managers
            commandManager = ManagerLocator.Get<CommandManager>();

            // Listen to Events
            panelSwap.AddListener(currentPanel =>
            {
                if (currentPanel == AbilityLoadoutPanelType.CharacterSelect)
                {
                    characterSelectPanel.enabled = true;
                    abilitySelectPanel.enabled = false;
                }
                else
                {
                    characterSelectPanel.enabled = false;
                    abilitySelectPanel.enabled = true;
                }
            });
            
            encounterStart.AddListener(() =>
            {
                Debug.Log("DRAFT AT START: APPEAR HERE");
                panelSwap.Invoke(AbilityLoadoutPanelType.CharacterSelect);
            });
            
            encounterWon.AddListener(() =>
            {
                Debug.Log("DRAFT AT END: APPEAR HERE");
            });
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<TurnQueueCreatedCommand>) OnEncounterStart);
            commandManager.ListenCommand((Action<EncounterWonCommand>) OnEncounterWon);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<TurnQueueCreatedCommand>) OnEncounterStart);
            commandManager.UnlistenCommand((Action<EncounterWonCommand>) OnEncounterWon);
        }

        #endregion
        
        #region Command Listeners

        private void OnEncounterStart(TurnQueueCreatedCommand cmd)
        {
            encounterStart.Invoke();
        }
        
        private void OnEncounterWon(EncounterWonCommand cmd)
        {
            encounterWon.Invoke();
        }
        
        #endregion

        #region Dialogue
        
        protected override void OnClose() {}

        protected override void OnPromote()
        {
            canvasGroup.interactable = true;
        }

        protected override void OnDemote()
        {
            canvasGroup.interactable = false;
        }
        
        #endregion
    }
}
