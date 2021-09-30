using Game;
using Managers;
using UI.Core;

namespace UI.CombatEndUI.AbilityUpgrading
{
    public class UpgradeButton : DialogueComponent<AbilityUpgradeDialogue>
    {
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
    
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.abilityUpgradeConfirm.Invoke();
            
            // TODO: Temporary encounter end. May change as more encounter reward dialogues are added.
            ManagerLocator.Get<GameManager>().EncounterEnded();
        
            manager.Pop();
        }
        
        #endregion
    }
}
