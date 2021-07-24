using Commands;

namespace Game.Commands
{
    /// <summary>
    /// Executed when the player has won a combat encounter.
    /// </summary>
    public class EncounterWonCommand : Command {}

    /// <summary>
    /// Executed when the player has lost a combat encounter.
    /// </summary>
    public class EncounterLostCommand : Command {}
}
