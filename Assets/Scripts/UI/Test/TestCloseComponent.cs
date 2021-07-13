namespace UI.Test
{
    public class TestCloseComponent : UIComponent<TestDialogue>
    {
        protected override void Subscribe()
        {
            
        }

        protected override void Unsubscribe()
        {
            
        }

        public void OnCloseClick()
        {
            manager.Pop();
        }
    }
}
