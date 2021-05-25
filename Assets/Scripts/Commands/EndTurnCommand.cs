using Managers;
using Units;

namespace Commands
{
    public class EndTurnCommand : Command
    {
        private TurnManager turnManager;
        
        public EndTurnCommand(IUnit unit) : base(unit)
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        public override void Queue() {}

        public override void Execute()
        {
            turnManager.EndTurn();
        }

        public override void Undo() {}
    }
}