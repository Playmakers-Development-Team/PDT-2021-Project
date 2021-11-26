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

        private void OnTurnStarted(GameDialogue.TurnInfo info)
        {
            SetInteractable(info.IsPlayer);
        }

        protected override void OnSelected()
        {
            base.OnSelected();
            
            dialogue.abilityDeselected.Invoke(dialogue.SelectedAbility);
            
            if (turnManager.ActingPlayerUnit != null && turnManager.IsMovementPhase())
                dialogue.modeChanged.Invoke(GameDialogue.Mode.Moving);
        }

        protected override void OnDeselected()
        {
            dialogue.modeChanged.Invoke(GameDialogue.Mode.Default);
        }
    }
}
