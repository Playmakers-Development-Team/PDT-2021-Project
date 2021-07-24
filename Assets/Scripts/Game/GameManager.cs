using Background;
using Commands;
using Game.Map;
using Managers;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : Manager
    {
        private CommandManager commandManager;
        private BackgroundManager backgroundManager;

        public EncounterData CurrentEncounterData { get; set; }
        
        public override void ManagerStart()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            backgroundManager = ManagerLocator.Get<BackgroundManager>();

            commandManager.ListenCommand<BackgroundCameraReadyCommand>(cmd => backgroundManager.Render());
        }

        // TODO: Replace with a scene transition manager
        private void ChangeScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex);
        }

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
    }
}
