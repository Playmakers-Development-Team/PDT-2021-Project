namespace UI
{
    public abstract class Panel<T> : Element where T : Dialogue
    {
        protected T dialogue;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            OnPanelAwake();
            Subscribe<T>();
        }

        protected abstract void OnPanelAwake();

        protected abstract void Subscribe<T1>();
    }
}
