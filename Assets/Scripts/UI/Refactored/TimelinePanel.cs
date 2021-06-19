using Managers;

namespace UI.Refactored
{
    public class TimelinePanel : Element
    {
        private TurnManager turnManager;

        protected override void OnAwake()
        {
            turnManager = ManagerLocator.Get<TurnManager>();
            
            manager.turnChanged.AddListener(OnTurnChanged);
        }

        private void OnTurnChanged() {}
    }
}
