using Commands;
using Game.Commands;
using Managers;
using UI.Commands;
using UI.Core;
using Units.Players;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CombatEndUI
{
    public class WinDialogue : Dialogue
    {
        [SerializeField] private Button gainButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button healButton;
        
        private CommandManager commandManager;
        private PlayerManager playerManager;

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();
            playerManager = ManagerLocator.Get<PlayerManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected override void OnClose() {}

        protected override void OnPromote()
        {
            gainButton.interactable = playerManager.AllowAbilityGain;
            upgradeButton.interactable = playerManager.AllowAbilityUpgrade;
            healButton.interactable = playerManager.AllowAbilityHeal;
        }
        
        protected override void OnDemote() {}

        public void Gain() => commandManager.ExecuteCommand(new SpawnAbilityLoadoutUICommand());
        
        public void Upgrade() => commandManager.ExecuteCommand(new SpawnAbilityUpgradeUICommand());

        public void Heal() => commandManager.ExecuteCommand(new HealPartyCommand());

    }
}
