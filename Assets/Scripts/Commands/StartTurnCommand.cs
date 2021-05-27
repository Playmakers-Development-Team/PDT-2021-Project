using Managers;
using Units;

namespace Commands
{
    public class StartTurnCommand : Command
    {
        private TurnManager turnManager;
        
        public StartTurnCommand(IUnit unit) : base(unit)
        {
            turnManager = ManagerLocator.Get<TurnManager>();

        }

        public override void Queue() {}

        public override void Execute()
        {
            turnManager.ShiftTurnQueue(turnManager.TurnIndex, turnManager.TurnIndex + 1);
        }

        public override void Undo() {}
    }
}