using Commands;
using Game.Map;

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

    /// <summary>
    /// Executed when map data is ready to be accessed.
    /// </summary>
    public class MapReadyCommand : Command
    {
        public MapData MapData { get; }

        public MapReadyCommand(MapData mapData) => MapData = mapData;
    }
}
