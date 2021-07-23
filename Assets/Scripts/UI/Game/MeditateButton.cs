namespace UI.Game
{
    public class MeditateButton : PanelButton
    {
        protected override void OnSelected()
        {
            dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
            dialogue.meditateConfirmed.Invoke(dialogue.SelectedUnit);
        }
    }
}
