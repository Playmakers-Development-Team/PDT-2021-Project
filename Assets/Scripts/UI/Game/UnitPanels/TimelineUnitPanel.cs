using Managers;

namespace UI
{
    internal class TimelineUnitPanel : UnitPanel
    {
        private TurnManager turnManager;
        
        protected override void OnComponentAwake()
        {
            // TODO: Ensure these are in the correct order...
            turnManager = ManagerLocator.Get<TurnManager>();
            
            manager.turnChanged.AddListener(OnTurnChanged);
            
            base.OnComponentAwake();
        }

        protected override void Disabled()
        {
            base.Disabled();
            manager.turnChanged.RemoveListener(OnTurnChanged);
        }

        private void OnTurnChanged()
        {
            selectedUnit = turnManager.ActingUnit;
            Redraw();
        }
    }
}
