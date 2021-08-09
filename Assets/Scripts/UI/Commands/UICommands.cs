using Units;
using Units.Commands;

namespace UI.Commands
{
    /// <summary>
    /// Called when the unit selection is performed, useful for things like testing.
    /// </summary>
    public class UIUnitSelectedCommand : UnitCommand
    {
        public UIUnitSelectedCommand(IUnit unit) : base(unit) {}
    }
}
