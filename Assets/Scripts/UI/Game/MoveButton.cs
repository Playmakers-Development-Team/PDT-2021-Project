using Managers;
using Turn;

namespace UI.Game
{
    public class MoveButton : PanelButton
    {
        private TurnManager turnManager;

        protected override void OnComponentAwake()
        {
            base.OnComponentAwake();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        protected override void OnSelected()
        {
            dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
            if (turnManager.ActingPlayerUnit != null && turnManager.IsMovementPhase())
                dialogue.moveButtonPressed.Invoke(true);
        }

        protected override void OnDeselected()
        {
            dialogue.moveButtonPressed.Invoke(false);
        }
    }
}
