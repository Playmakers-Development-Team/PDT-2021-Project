using Managers;
using Units;

namespace Commands
{
    public class UnitSelectedCommand : Command
    {
        //Currently unused idk if it should be delegate or command.
        public IUnit currentUnit;
        
        public static IUnit CurrentUnit => ManagerLocator.Get<TurnManager>().CurrentUnit;
        
        public delegate void StartTurnUpdate(IUnit unit);
        
        public static StartTurnUpdate UnitSelect;
        
        public UnitSelectedCommand(IUnit unit) : base(unit) 
        {
            currentUnit = unit;
        }

        public override void Queue() {}

        public override void Execute() 
        {
            UnitSelect(currentUnit);
        }

        public override void Undo() {}
    }
}