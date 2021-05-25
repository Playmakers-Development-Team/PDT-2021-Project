using Managers;
using Units;

namespace Commands
{
    public class UnitSelectedCommand : Command
    {
        //Currently unused idk if it should be delegate or command.
        public IUnit CurrentUnit;
        public static IUnit GetCurrentUnit{get {return ManagerLocator.Get<TurnManager>().CurrentUnit;}}
        public delegate void StartTurnUpdate(IUnit unit);
        public static StartTurnUpdate UnitSelect;
        public UnitSelectedCommand(IUnit unit) : base(unit) 
        {
            CurrentUnit = unit;
        }

        public override void Queue() {}

        public override void Execute() 
        {
            UnitSelect(CurrentUnit);
        }

        public override void Undo() {}
    }
}