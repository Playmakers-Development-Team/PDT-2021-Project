using UI.Core;

namespace UI.General
{
    public class CloseButton : DialogueComponent<Dialogue>
    {
        #region UIComponent
        
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        #endregion
        
        
        #region Listeners
        
        public void OnPressed()
        {
            manager.Pop();
        }
        
        #endregion
    }
}
