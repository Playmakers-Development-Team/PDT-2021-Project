using System;
using Commands;
using Managers;
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
        
        private CommandManager commandManager;

        #region Monobehavior Functions

        private void Awake()
        {
            Instantiate(dialogue, transform);
            
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
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers);
            
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.ListenCommand((Action<SpawnAbilityUpgradeUICommand>) SpawnAbilityUpgrade);
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

        #endregion
    }
}
