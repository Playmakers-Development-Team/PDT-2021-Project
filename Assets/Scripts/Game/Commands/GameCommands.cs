using Commands;
using Game.Map;
using UnityEngine;

namespace Game.Commands
{
    /// <summary>
    /// Should be executed when the player wants to play the game, like when pressing the play button.
    /// </summary>
    public class PlayGameCommand : Command {}

    /// <summary>
    /// Should be executed when the player wants to continue a saved game, like when pressing the continue button.
    /// </summary>
    public class ContinueGameCommand : Command {}

    /// <summary>
    /// Execute to go back to the main menu.
    /// </summary>
    public class MainMenuCommand : Command {}

    /// <summary>
    /// Executed when we are finished with this encounter, we picked all our upgrades and stuff,
    /// and that we should load the next encounter.
    /// </summary>
    public class EndEncounterCommand : Command {}

    /// <summary>
    /// Executed when the player has won a combat encounter.
    /// </summary>
    public class EncounterWonCommand : Command {}

    /// <summary>
    /// Executed when the player has lost a combat encounter.
    /// </summary>
    public class EncounterLostCommand : Command {}

    /// <summary>
    /// Restart the encounter
    /// </summary>
    public class RestartEncounterCommand : Command {}

    /// <summary>
    /// Launch a tutorial with the appropriate tutorial object, which is usually a tutorial dialogue
    /// </summary>
    public class LaunchTutorialCommand : Command
    {
        public GameObject tutorialObject;

        public LaunchTutorialCommand(GameObject tutorialObject) => this.tutorialObject = tutorialObject;
    }

    /// <summary>
    /// Executed when map data is ready to be accessed.
    /// </summary>
    public class MapReadyCommand : Command
    {
        public MapData MapData { get; }

        public MapReadyCommand(MapData mapData) => MapData = mapData;
    }
}
