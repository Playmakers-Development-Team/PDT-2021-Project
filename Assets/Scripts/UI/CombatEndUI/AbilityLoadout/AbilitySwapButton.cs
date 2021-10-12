using Commands;
using Game;
using Game.Commands;
using Managers;
using UI.Core;

namespace UI.CombatEndUI.AbilityLoadout
{
    public class AbilitySwapButton : DialogueComponent<AbilityLoadoutDialogue>
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
            dialogue.abilitySwapConfirm.Invoke();
            
            manager.Pop();
            
            commandManager.ExecuteCommand(new EndEncounterCommand());
        }
        
        #endregion
    }
}
