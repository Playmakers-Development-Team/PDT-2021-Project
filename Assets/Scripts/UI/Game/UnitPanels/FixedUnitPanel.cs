namespace UI.Game.UnitPanels
{
    internal class FixedUnitPanel : UnitPanel
    {
        #region UIComponent
        
        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        #endregion
        
        
        #region Listeners
        
        private void OnTurnStarted(GameDialogue.TurnInfo turnInfo)
        {
            unitInfo = turnInfo.CurrentUnit;
            Redraw();
        }
        
        #endregion
    }
}
