using Managers;
using Turn;
using Units;

namespace Tests.Constraints
{
    public class UnitTurnConstraint : AbstractUnitConstraint
    {
        protected override bool IsCorrectUnit(IUnit unit) => 
            ManagerLocator.Get<TurnManager>().ActingUnit == unit;
    }
}
