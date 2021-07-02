using Managers;

namespace UI
{
    public class TimelineUnitPanel : UnitPanel
    {
        private TurnManager turnManager;
        
        protected override void OnAwake()
        {
            base.OnAwake();

            turnManager = ManagerLocator.Get<TurnManager>();
            
            manager.turnChanged.AddListener(OnTurnChanged);
        }

        protected override void Disabled()
        {
            base.Disabled();
            manager.turnChanged.RemoveListener(OnTurnChanged);
        }

        private void OnTurnChanged()
        {
            selectedUnit = turnManager.ActingPlayerUnit;
            Redraw();
        }
    }
}
