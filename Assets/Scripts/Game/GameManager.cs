using Background;
using Commands;
using Game.Commands;
using Game.Map;
using Managers;
using Turn.Commands;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : Manager
    {
        private CommandManager commandManager;
        private BackgroundManager backgroundManager;

        public EncounterData CurrentEncounterData { get; set; }
        public MapData CurrentMapData { get; set; }

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            backgroundManager = ManagerLocator.Get<BackgroundManager>();

            commandManager.ListenCommand<BackgroundCameraReadyCommand>(cmd => backgroundManager.Render());
        }

        // TODO: Replace with a scene transition manager
        private static void ChangeScene(int buildIndex) =>
            SceneManager.LoadScene(buildIndex);

        private static void ChangeScene(SceneReference sceneReference) =>
            SceneManager.LoadScene(sceneReference);

        public void LoadEncounter(EncounterData encounterData)
        {
            // Only listen to the end of an encounter if it was loaded from the map scene
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(cmd => EncounterWon());
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(cmd => EncounterLost());
            
            CurrentEncounterData = encounterData;

            ChangeScene(encounterData.encounterScene);
        }

        public void LoadMap()
        {
            // TODO: Magic number
            ChangeScene(1);
        }
        
        private void EncounterLost()
        {
            // TODO: Go back to the main menu?
            LoadMap();
            
            commandManager.ExecuteCommand(new EncounterLostCommand());
        }

        private void EncounterWon()
        {
            LoadMap();

            CurrentMapData.EncounterCompleted(CurrentEncounterData);
            
            commandManager.ExecuteCommand(new EncounterWonCommand());
        }
    }
}
