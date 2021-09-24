using Game;
using Managers;
using UI.Core;

namespace UI.AbilityLoadout
{
    public class AbilitySwapButton : DialogueComponent<AbilityLoadoutDialogue>
    {
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
    
        #region Listeners
        
        public void OnPressed()
        {
            dialogue.abilitySwap.Invoke();
            
            // TODO: Temporary encounter end. May change as more encounter reward dialogues are added.
            ManagerLocator.Get<GameManager>().EncounterEnded();
        
            manager.Pop();
        }
        
        #endregion
    }
}
