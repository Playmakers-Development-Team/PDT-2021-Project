namespace UI.Game
{
    public class MoveButton : PanelButton
    {
        protected override void OnSelected()
        {
            dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
        }
    }
}
