using Commands;
using Managers;
using UI.Commands;
using UI.Core;
using UnityEngine;

namespace UI.CombatEndUI
{
    public class WinDialogue : Dialogue
    {
        [SerializeField] private GameObject abilityLoadoutDialogue;
        private CommandManager commandManager;

        protected override void OnDialogueAwake()
        {
            base.OnDialogueAwake();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected override void OnClose()
        {
        }

        protected override void OnPromote()
        {
        
        }

        protected override void OnDemote()
        {
        }

        public void Gain() => commandManager.ExecuteCommand(new SpawnAbilityLoadoutUICommand());
    
    }
}
