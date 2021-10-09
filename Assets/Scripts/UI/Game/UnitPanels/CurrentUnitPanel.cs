namespace UI.Game.UnitPanels
{
    internal class CurrentUnitPanel : UnitPanel
    {
        #region UIComponent
        
        protected override void Subscribe()
        {
            base.Subscribe();
            dialogue.turnStarted.AddListener(OnTurnStarted);
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        #endregion
        
        
        #region Listeners
        
        private void OnTurnStarted(GameDialogue.TurnInfo turnInfo)
        {
            unitInfo = turnInfo.CurrentUnitInfo;
            Redraw();
        }
        
        #endregion
    }
}
