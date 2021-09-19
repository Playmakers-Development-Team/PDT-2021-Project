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
        
            manager.Pop();
        }
        
        #endregion
    }
}
