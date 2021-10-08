using Commands;
using Game;
using Game.Commands;
using Managers;
using UI.Core;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class UpgradeButton : DialogueComponent<AbilityUpgradeDialogue>
    {
        private CommandManager commandManager;

        #region Monobehavior Events

        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        #endregion
        
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
    
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.abilityUpgradeConfirm.Invoke();
            
            manager.Pop();
            
            commandManager.ExecuteCommand(new EndEncounterCommand());
        }
        
        #endregion
    }
}
