using System;
using System.Collections.Generic;
using System.Linq;
using Background;
using Commands;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Map;
using Managers;
using Turn.Commands;
using UnityEngine;
using Turn;
using Units.Players;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game
{
    public class GameManager : Manager
    {
        private enum EncounterResult
        {
            Won, Lost
        }
        
        private CommandManager commandManager;
        private BackgroundManager backgroundManager;
        private PlayerManager playerManager;
        private TurnManager turnManager;

        /// <summary>
        /// Keeps track of all the levels that we have already seen in each game's run. We don't want to see
        /// the same level more than once.
        /// </summary>
        private HashSet<SceneReference> visitedLevels = new HashSet<SceneReference>();
        /// <summary>
        /// How long should we delay the game before we end the encounter after losing one.
        /// </summary>
        private const float delayAfterLostEncounter = 2f;

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
            commandManager.ListenCommand<RestartEncounterCommand>(cmd => RestartEncounter());
            // TODO keep map information somewhere and call run linear map directly
            commandManager.ListenCommand<PlayGameCommand>(cmd => ChangeScene("Assets/Scenes/Design/Playtest MVP-1/INTRO/INTRO 1.unity"));
            
            // Automatically end the encounter if there are no players remaining after a few seconds
            commandManager.ListenCommand<NoRemainingPlayerUnitsCommand>(async (cmd) =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delayAfterLostEncounter));
                commandManager.ExecuteCommand(new EndEncounterCommand());
            });
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
                EncounterResult encounterResult = EncounterEnded();

                if (encounterResult == EncounterResult.Lost)
                {
                    // For now, reset the scene
                    commandManager.ExecuteCommand(new RestartEncounterCommand());
                }
                else if (encounterNode.ConnectedNodes.Count > 0)
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
            
            ResetVisitedLevels();
        }

        public void StopLinearMap() => CurrentMapData = null;

        private static void ChangeScene(string scene) =>
            SceneManager.LoadScene(scene);
        
        private static async UniTask ChangeSceneAsync(string scene) =>
            await SceneManager.LoadSceneAsync(scene);

        /// <summary>
        /// Same thing as <see cref="LoadEncounter"/>, but async. Remember that once this is called, the scene
        /// is loaded in background rather than instantly.
        /// </summary>
        public async UniTask LoadEncounterAsync(EncounterData encounterData, bool forceChangeScene = true)
        {
            Debug.Log($"Load asynchronously next encounter {encounterData.name}");
            SceneReference nextScene = PullValidEncounterScene(encounterData, forceChangeScene);
            CurrentEncounterData = encounterData;
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
            SceneReference nextScene = PullValidEncounterScene(encounterData, forceChangeScene);
            CurrentEncounterData = encounterData;
            LoadEncounterScene(nextScene, forceChangeScene);
        }

        private void LoadEncounterScene(SceneReference nextScene, bool forceChangeScene = true)
        {
            encounterLoadedFromMap = true;
            visitedLevels.Add(nextScene);
            
            if (forceChangeScene || SceneManager.GetActiveScene().path != nextScene.ScenePath)
                ChangeScene(nextScene);
        }
        
        private async UniTask LoadEncounterSceneAsync(SceneReference nextScene, bool forceChangeScene = true)
        {
            encounterLoadedFromMap = true;
            visitedLevels.Add(nextScene);
            
            if (forceChangeScene || SceneManager.GetActiveScene().path != nextScene.ScenePath)
                await ChangeSceneAsync(nextScene);
        }

        private SceneReference PullValidEncounterScene(EncounterData encounterData, bool forceChangeScene = true)
        {
            var currentScene = SceneManager.GetActiveScene().path;

            if (encounterData.GetAllPossibleScenes().Count() == 1)
            {
                SceneReference onlyScene = encounterData.PullEncounterScene();

                if (currentScene == onlyScene && forceChangeScene)
                {
                    throw new Exception(
                        $"We cannot go to another scene since the next encounter {encounterData.name} only has one scene, and it is the current scene!");
                }
                else
                {
                    return onlyScene;
                }
            }

            var foundScene = encounterData.PullEncounterScenes()
                .Where((s) => !visitedLevels.Contains(s))
                .FirstOrDefault((s) => s != currentScene);

            if (foundScene == null)
            {
                Debug.LogWarning($"All scenes from the next encounter {encounterData.name} has been visited. Hence, we're going to get a random one anyways!");
                return encounterData.PullEncounterScene();
            }
            else
            {
                return foundScene;
            }
        }

        public void LoadMap()
        {
            if (CurrentMapData != null && !string.IsNullOrEmpty(CurrentMapData.mapScene?.ScenePath))
                ChangeScene(CurrentMapData.mapScene);
        }

        private void RestartEncounter()
        {
            ChangeScene(SceneManager.GetActiveScene().path);
        }
        
        private void EncounterLost()
        {
            // TODO: Exporting data here is temporary, should probably delete any saved data.
            // playerManager.ExportData();
            
            // TODO: Go back to the main menu?
            // LoadMap();
            
            commandManager.ExecuteCommand(new EncounterLostCommand());
        }

        private void EncounterWon()
        {
            playerManager.ExportData();
            
            // LoadMap();

            CurrentMapData.EncounterCompleted(CurrentEncounterData);
            
            commandManager.ExecuteCommand(new EncounterWonCommand());
        }

        private EncounterResult EncounterEnded()
        {
            // if (!encounterLoadedFromMap)
            //     return;

            if (turnManager.HasPlayerUnitInQueue())
            {
                EncounterWon();
                return EncounterResult.Won;
            }
            else
            {
                EncounterLost();
                return EncounterResult.Lost;
            }
        }

        public void SetEndEncounterToLoadMap()
        {
            commandManager.ListenCommand<EndEncounterCommand>((cmd) =>
            {
                EncounterEnded();
                LoadMap();
            });
        }
        
        /// <summary>
        /// Should be called between each run of the game. Allows all levels to be visited again.
        /// <see cref="visitedLevels"/> 
        /// </summary>
        private void ResetVisitedLevels()
        {
            visitedLevels.Clear();
        }
    }
}
