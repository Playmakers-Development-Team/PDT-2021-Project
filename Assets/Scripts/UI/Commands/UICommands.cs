using Commands;
using UI.AbilityLoadout.Abilities;
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

    /// <summary>
    /// Executed when an ability button is clicked for new ability selection
    /// </summary>
    public class AbilitySelectedCommand : Command
    {
        public AbilityButton AbilityButton { get; }
        public bool IsNewAbility { get; }

        protected internal AbilitySelectedCommand(AbilityButton abilityButton, bool isNewAbility)
        {
            AbilityButton = abilityButton;
            IsNewAbility = isNewAbility;
        }
    }
}
