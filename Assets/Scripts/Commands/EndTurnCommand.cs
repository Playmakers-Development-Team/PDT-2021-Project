using System;
using Managers;
using Units;

namespace Commands
{
    public class EndTurnCommand : Command
    {

        private event Action<EndTurnCommand> turnEnded;  
        private TurnManager turnManager;
        
        public EndTurnCommand(IUnit unit) : base(unit)
        {
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        public override void Queue() {}

        public override void Execute()
        {
            turnManager.NextTurn();
            turnEnded?.Invoke(this);
        }

        public override void Undo() {}
    }
}