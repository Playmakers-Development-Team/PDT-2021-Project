using System;
using Commands;
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
        
        [SerializeField] private GameObject abilityLoadoutDialogue;
        [SerializeField] private GameObject tutorialDialogue;
        [SerializeField] private GameObject abilityUpgradeDialogue;
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
            
            if (tutorialDialogue)
                LoadObject(tutorialDialogue);

            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.ListenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers); 
            
            commandManager.ListenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.ListenCommand((Action<SpawnAbilityUpgradeUICommand>) SpawnAbilityUpgrade);
            
            commandManager.ListenCommand<RoundZeroCommand>(OnRoundZeroCommand);
            commandManager.ListenCommand<StartRoundCommand>(OnStartRoundCommand);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers);
            
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.ListenCommand((Action<SpawnAbilityUpgradeUICommand>) SpawnAbilityUpgrade);
            
            commandManager.UnlistenCommand<StartRoundCommand>(OnStartRoundCommand);
        }

        #endregion

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
        
        private void OnRoundZeroCommand(RoundZeroCommand cmd)
        {
            ShowStartRoundBanner(0);
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
