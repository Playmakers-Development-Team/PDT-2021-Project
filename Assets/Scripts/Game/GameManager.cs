using Background;
using Commands;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Map;
using Managers;
using Turn.Commands;
using UnityEngine;
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

        public async UniTaskVoid RunLinearMap(MapData mapDataAsset)
        {
            // If we are already running map, don't run again
            if (CurrentMapData != null)
            {
                Debug.LogWarning($"Skipping linear map run because we are already running a map!");
                return;
            }
            
            Debug.Log("Running linear map...");

            CurrentMapData = Object.Instantiate(mapDataAsset);
            CurrentMapData.Initialise();
            EncounterNode encounterNode = CurrentMapData.GetFirstAvailableNodeOrNull();
            
            if (encounterNode == null)
            {
                Debug.LogError("Cannot run linear map, map does not have a starting node!");
                return;
            }
            
            var encounterData = encounterNode.EncounterData;

            while (CurrentMapData != null && encounterData != null)
            {
                LoadEncounter(encounterData, false);
                await commandManager.WaitForCommand<EndEncounterCommand>();

                if (encounterNode.ConnectedNodes.Count > 0)
                {
                    // Pick a random connected node
                    int randomIndex = UnityEngine.Random.Range(0, encounterNode.ConnectedNodes.Count);
                    encounterNode = encounterNode.ConnectedNodes[randomIndex];
                    
                    if (CurrentEncounterData == encounterNode.EncounterData)
                        Debug.LogWarning($"The next encounter for {CurrentEncounterData} is itself. Is something wrong with the map?");
                    
                    encounterData = encounterNode.EncounterData;
                }
                else
                {
                    break;
                }
            }
        }

        public void StopLinearMap() => CurrentMapData = null;

        private static void ChangeScene(SceneReference sceneReference) =>
            SceneManager.LoadScene(sceneReference);
        
        private static async UniTask ChangeSceneAsync(SceneReference sceneReference) =>
            await SceneManager.LoadSceneAsync(sceneReference);

        /// <summary>
        /// Same thing as <see cref="LoadEncounter"/>, but async. Remember that once this is called, the scene
        /// is loaded in background rather than instantly.
        /// </summary>
        public async UniTask LoadEncounterAsync(EncounterData encounterData, bool forceChangeScene = true)
        {
            Debug.Log($"Load asynchronously next encounter {encounterData.name}");
            CurrentEncounterData = encounterData;
            SceneReference nextScene = encounterData.PullEncounterScene();
            await LoadEncounterSceneAsync(nextScene, forceChangeScene);
        }

        /// <summary>
        /// Load the encounter from the encounter data
        /// </summary>
        /// <param name="encounterData">The encounter data which contains one or more scenes</param>
        /// <param name="forceChangeScene">Do we always have to change the scene regardless if we are already in the correct scene?</param>
        public void LoadEncounter(EncounterData encounterData, bool forceChangeScene = true)
        {
            Debug.Log($"Load next encounter {encounterData.name}");
            CurrentEncounterData = encounterData;
            SceneReference nextScene = encounterData.PullEncounterScene();
            LoadEncounterScene(nextScene, forceChangeScene);
        }

        private void LoadEncounterScene(SceneReference nextScene, bool forceChangeScene = true)
        {
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(cmd => EncounterWon());
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(cmd => EncounterLost());
            
            if (forceChangeScene || SceneManager.GetActiveScene().path != nextScene.ScenePath)
                ChangeScene(nextScene);
        }
        
        private async UniTask LoadEncounterSceneAsync(SceneReference nextScene, bool forceChangeScene = true)
        {
            // Only listen to the end of an encounter if it was loaded from the map scene
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(cmd => EncounterWon());
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(cmd => EncounterLost());
            
            if (forceChangeScene || SceneManager.GetActiveScene().path != nextScene.ScenePath)
                await ChangeSceneAsync(nextScene);
        }

        public void LoadMap()
        {
            if (CurrentMapData != null && !string.IsNullOrEmpty(CurrentMapData.mapScene?.ScenePath))
                ChangeScene(CurrentMapData.mapScene);
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
