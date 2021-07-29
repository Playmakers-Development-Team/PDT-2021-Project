namespace UI.Game.UnitPanels
{
    internal class SelectedUnitPanel : UnitPanel
    {
        #region UIComponent
        
        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
            dialogue.unitSelected.AddListener(OnUnitSelected);
            dialogue.unitDeselected.AddListener(OnUnitDeselected);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
            dialogue.unitSelected.RemoveListener(OnUnitSelected);
            dialogue.unitDeselected.RemoveListener(OnUnitDeselected);
        }

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            Hide();
        }

        #endregion
        

        #region Listeners
        
        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            Redraw();
        }

        private void OnUnitSelected(GameDialogue.UnitInfo unit)
        {
            unitInfo = unit;
            
            Show();
            Redraw();
        }

        private void OnUnitDeselected()
        {
            unitInfo = null;
            Hide();
        }
        
        #endregion
        
        
        #region Drawing
        
        private void Hide() => canvas.enabled = false;

        private void Show() => canvas.enabled = true;
        
        #endregion
    }
}
