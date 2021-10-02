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
        
        //TODO: THIS WILL LATER BE MOVED TO THE END SCREEN DIALOGUE SCRIPT
        [SerializeField] private GameObject abilityLoadoutDialogue;
        [SerializeField] private GameObject tutorialDialogue;
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
            //TODO: MOVE TO ENDSCREENDIALOGUE
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.ListenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.ListenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers); 
        }

        private void OnDisable()
        {
            //TODO: MOVE TO ENDSCREENDIALOGUE
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
            commandManager.UnlistenCommand((Action<NoRemainingPlayerUnitsCommand>) OnNoRemainingPlayers); 
        }

        #endregion

        public void LoadObject(GameObject obj) => Instantiate(obj, transform);

        #region MOVE TO ENDSCREENDIALOGUE

        //TODO: THIS WILL LATER BE MOVED TO BE CALLED IN THE END SCREEN DIALOGUE SCRIPT
        private void OnNoRemainingEnemies(NoRemainingEnemyUnitsCommand cmd)
        {
            if (winDialogue)
                LoadObject(winDialogue);
        }
        
        private void SpawnAbilityLoadout(SpawnAbilityLoadoutUICommand cmd)
        {
            // Some levels may not have the ability loadout
            if (abilityLoadoutDialogue)
                LoadObject(abilityLoadoutDialogue);
        }

        private void OnNoRemainingPlayers(NoRemainingPlayerUnitsCommand cmd)
        {
            if (loseDialogue)
                LoadObject(loseDialogue); 
        }

        #endregion
    }
}
