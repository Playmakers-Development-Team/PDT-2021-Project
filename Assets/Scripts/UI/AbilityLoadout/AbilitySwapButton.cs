using Commands;
using Game.Commands;
using Managers;
using UI.Core;

namespace UI.AbilityLoadout
{
    public class AbilitySwapButton : DialogueComponent<AbilityLoadoutDialogue>
    {
        private CommandManager commandManager;
        
        #region UIComponent

        protected override void OnComponentAwake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
    
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.abilitySwap.Invoke();
        
            manager.Pop();
            
            commandManager.ExecuteCommand(new EndEncounterCommand());
        }
        
        #endregion
    }
}
