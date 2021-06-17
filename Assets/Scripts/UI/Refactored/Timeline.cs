using Managers;

namespace UI.Refactored
{
    public class Timeline : Element
    {
        private TurnManager turnManager;

        protected override void OnAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            
            manager.turnChanged.AddListener(OnTurnChanged);
        }

        private void OnTurnChanged()
        {
            Refresh();
        }

        protected override void Refresh() {}
    }
}
