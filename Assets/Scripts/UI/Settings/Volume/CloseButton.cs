namespace UI.Settings.Volume
{
    public class CloseButton : UIComponent<Dialogue>
    {
        protected override void Subscribe() {}

        protected override void Unsubscribe() {}

        public void OnPressed()
        {
            manager.Pop();
        }
    }
}
