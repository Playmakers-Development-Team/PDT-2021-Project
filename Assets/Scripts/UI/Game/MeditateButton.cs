namespace UI.Game
{
    public class MeditateButton : PanelButton
    {
        protected override void OnSelected()
        {
            base.OnSelected();
            dialogue.meditateConfirmed.Invoke(dialogue.SelectedUnit);
        }
    }
}
