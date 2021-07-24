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
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(cmd => EncounterWon());
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(cmd => EncounterLost());
        }

        // TODO: Replace with a scene transition manager
        private void ChangeScene(int buildIndex) => SceneManager.LoadScene(buildIndex);

        public void ToEncounter(EncounterData encounterData)
        {
            CurrentEncounterData = encounterData;
            
            // TODO: Magic number
            ChangeScene(1);
        }

        public void ToMap()
        {
            // TODO: Magic number
            ChangeScene(0);
        }
        
        private void EncounterLost()
        {
            // TODO: Go back to the main menu
            ToMap();
            
            commandManager.ExecuteCommand(new EncounterLostCommand());
        }

        private void EncounterWon()
        {
            ToMap();

            CurrentMapData.EncounterCompleted(CurrentEncounterData);
            
            commandManager.ExecuteCommand(new EncounterWonCommand());
        }
    }
}
