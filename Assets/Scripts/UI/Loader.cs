using System;
using Commands;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Managers;
using TMPro;
using Turn.Commands;
using UI.Commands;
using UnityEngine;

namespace UI
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private GameObject dialogue;
        
        [SerializeField] private GameObject tutorialDialogue;
        [SerializeField] private GameObject abilityLoadoutDialogue;
        [SerializeField] private GameObject abilityUpgradeDialogue;
        [SerializeField] private GameObject abilityHealDialogue;
        [SerializeField] private GameObject loseDialogue;
        [SerializeField] private GameObject winDialogue;
        [SerializeField] private GameObject endOfRoundBannerPrefab;
        private GameObject endOfRoundBanner;
        private float bannerActiveTime = 2.0f;
        
        private CommandManager commandManager;

        #region Monobehavior Functions

        private void Awake()
        {
            Instantiate(dialogue, transform);
            
            endOfRoundBanner = Instantiate(endOfRoundBannerPrefab, transform);
            endOfRoundBanner.SetActive(false);

            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.ListenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers); 
            
            commandManager.ListenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.ListenCommand((Action<SpawnAbilityUpgradeUICommand>) SpawnAbilityUpgrade);
            commandManager.ListenCommand<HealPartyCommand>(SpawnHeal);
            
            commandManager.ListenCommand<RoundZeroCommand>(OnRoundZeroCommand);
            commandManager.ListenCommand<StartRoundCommand>(OnStartRoundCommand);
            
            commandManager.ListenCommand<LaunchTutorialCommand>(OnLaunchTutorial);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers);
            
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.UnlistenCommand((Action<SpawnAbilityUpgradeUICommand>) SpawnAbilityUpgrade);
            
            commandManager.UnlistenCommand<StartRoundCommand>(OnStartRoundCommand);
            
            commandManager.UnlistenCommand<LaunchTutorialCommand>(OnLaunchTutorial);
        }

        #endregion

        private void OnLaunchTutorial(LaunchTutorialCommand cmd)
        {
            if (!cmd.tutorialObject)
                return;

            // We just force change the tutorial dialogue, should work well enough
            tutorialDialogue = cmd.tutorialObject;
            Debug.Log($"Replacing tutorial with {tutorialDialogue.name}");
        }

        private async void ShowTutorial()
        {
            await UniTask.Yield();
            await UniTask.WaitWhile(() => endOfRoundBanner.gameObject.activeSelf);
            
            if (tutorialDialogue)
                LoadObject(tutorialDialogue);
        }

        public void LoadObject(GameObject obj) => Instantiate(obj, transform);

        #region MOVE TO ENDSCREENDIALOGUE
        
        private void OnNoRemainingEnemies(NoRemainingEnemyUnitsCommand cmd)
        {
            if (winDialogue)
                LoadObject(winDialogue);
        }
        
        private void OnNoRemainingPlayers(NoRemainingPlayerUnitsCommand cmd)
        {
            if (loseDialogue)
                LoadObject(loseDialogue); 
        }
        
        private void SpawnAbilityLoadout(SpawnAbilityLoadoutUICommand cmd)
        {
            if (abilityLoadoutDialogue)
                LoadObject(abilityLoadoutDialogue);
        }

        private void SpawnAbilityUpgrade(SpawnAbilityUpgradeUICommand cmd)
        {
            if (abilityUpgradeDialogue)
                LoadObject(abilityUpgradeDialogue);
        }

        private void SpawnHeal(HealPartyCommand cmd)
        {
            if (abilityHealDialogue)
                LoadObject(abilityHealDialogue);
        }
        
        private void OnRoundZeroCommand(RoundZeroCommand cmd)
        {
            ShowStartRoundBanner(0);
            
            // NOTE: If you are moving this somewhere else, put the code below into its own function in the Loader.cs
            ShowTutorial();
        }
        
        private void OnStartRoundCommand(StartRoundCommand cmd)
        {
            ShowStartRoundBanner(cmd.RoundCount);
        }

        private void ShowStartRoundBanner(int roundCount)
        {
            endOfRoundBanner.SetActive(true);
            endOfRoundBanner.GetComponentInChildren<TextMeshProUGUI>().text = "Round " + roundCount;
            
            Invoke("HideStartRoundBanner", bannerActiveTime);
        }

        private void HideStartRoundBanner() => endOfRoundBanner.SetActive(false);

        #endregion
    }
}
