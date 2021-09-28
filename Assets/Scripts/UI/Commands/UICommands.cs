using Commands;
using UI.CombatEndUI.AbilityLoadout.Abilities;
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
    /// Executed to spawn the ability loadout dialogue
    /// </summary>
    public class SpawnAbilityLoadoutUICommand : Command {}
    
    /// <summary>
    /// Executed to spawn the ability upgrade dialogue
    /// </summary>
    public class SpawnAbilityUpgradeUICommand : Command {}

    /// <summary>
    /// Executed when an the ability loadout or upgrade dialogue is ready
    /// i.e. when Awake() finishes in AbilityLoadoutDialogue/AbilityUpgradeDialogue
    /// </summary>
    public class AbilityRewardDialogueReadyCommand : Command {}

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
