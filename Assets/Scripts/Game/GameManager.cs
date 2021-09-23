using Background;
using Commands;
using Game.Commands;
using Game.Map;
using Managers;
using Turn;
using Units.Players;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : Manager
    {
        private CommandManager commandManager;
        private BackgroundManager backgroundManager;
        private PlayerManager playerManager;
        private TurnManager turnManager;

        public EncounterData CurrentEncounterData { get; set; }
        public MapData CurrentMapData { get; set; }

        /// <summary>
        /// False if the encounter was loaded directly in the editor. Used to make sure we only
        /// return to the map scene if that's where we came from.
        /// </summary>
        private bool encounterLoadedFromMap = false;

        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            backgroundManager = ManagerLocator.Get<BackgroundManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();
            turnManager = ManagerLocator.Get<TurnManager>();

            commandManager.ListenCommand<BackgroundCameraReadyCommand>(cmd => backgroundManager.Render());
        }

        // TODO: Replace with a scene transition manager
        private static void ChangeScene(int buildIndex) =>
            SceneManager.LoadScene(buildIndex);

        private static void ChangeScene(SceneReference sceneReference) =>
            SceneManager.LoadScene(sceneReference);

        public void LoadEncounter(EncounterData encounterData)
        {
            encounterLoadedFromMap = true;
            
            CurrentEncounterData = encounterData;

            ChangeScene(encounterData.encounterScene);
        }

        private void LoadMap()
        {
            // TODO: Magic number
            ChangeScene(1);
        }
        
        private void EncounterLost()
        {
            // TODO: Exporting data here is temporary, should probably delete any saved data.
            playerManager.ExportData();
            
            // TODO: Go back to the main menu?
            LoadMap();
            
            commandManager.ExecuteCommand(new EncounterLostCommand());
        }

        private void EncounterWon()
        {
            playerManager.ExportData();
            
            LoadMap();

            CurrentMapData.EncounterCompleted(CurrentEncounterData);
            
            commandManager.ExecuteCommand(new EncounterWonCommand());
        }

        public void EncounterEnded()
        {
            if (!encounterLoadedFromMap)
                return;

            if (turnManager.HasPlayerUnitInQueue())
                EncounterWon();
            else 
                EncounterLost();
        }
    }
}
