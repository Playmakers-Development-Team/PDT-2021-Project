namespace UI.Game.UnitPanels
{
    internal class FixedUnitPanel : UnitPanel
    {
        protected override void Subscribe()
        {
            dialogue.turnStarted.AddListener(OnTurnStarted);
        }

        protected override void Unsubscribe()
        {
            dialogue.turnStarted.RemoveListener(OnTurnStarted);
        }

        private void OnTurnStarted(GameDialogue.TurnInfo turnInfo)
        {
            unitInfo = turnInfo.CurrentUnit;
            Redraw();
        }
    }
}
