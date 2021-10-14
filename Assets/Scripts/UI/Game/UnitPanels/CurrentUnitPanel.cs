using Managers;
using Turn;
using Units.Players;

namespace UI.Game.UnitPanels
{
    internal class CurrentUnitPanel : UnitPanel
    {
        #region UIComponent
        
        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.turnManipulated.AddListener(OnTurnManipulated);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        #endregion
        
        
        #region Listeners
        
        private void OnTurnStarted(GameDialogue.TurnInfo turnInfo)
        {
            unitInfo = turnInfo.CurrentUnitInfo;
            Redraw();
        }
        private void OnTurnManipulated(GameDialogue.UnitInfo unitInfo)
        {
            if (unitInfo.Unit.Equals(typeof(PlayerUnit)))
            {
                this.unitInfo = unitInfo;
                Redraw();
            }
        }
        
        #endregion
    }
}
