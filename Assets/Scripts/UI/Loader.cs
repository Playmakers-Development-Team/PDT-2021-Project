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
        
        private CommandManager commandManager;

        #region Monobehavior Functions

        private void Awake()
        {
            Instantiate(dialogue, transform);

            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            //TODO: MOVE TO ENDSCREENDIALOGUE
            commandManager.ListenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.ListenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
        }

        private void OnDisable()
        {
            //TODO: MOVE TO ENDSCREENDIALOGUE
            commandManager.UnlistenCommand((Action<NoRemainingEnemyUnitsCommand>) OnNoRemainingEnemies);
            commandManager.UnlistenCommand((Action<SpawnAbilityLoadoutUICommand>) SpawnAbilityLoadout);
        }

        #endregion

        public void LoadObject(GameObject obj) => Instantiate(obj, transform);

        #region MOVE TO ENDSCREENDIALOGUE

        //TODO: THIS WILL LATER BE MOVED TO BE CALLED IN THE END SCREEN DIALOGUE SCRIPT
        private void OnNoRemainingEnemies(NoRemainingEnemyUnitsCommand cmd)
        {
            commandManager.ExecuteCommand(new SpawnAbilityLoadoutUICommand());
        }
        
        private void SpawnAbilityLoadout(SpawnAbilityLoadoutUICommand cmd)
        {
            LoadObject(abilityLoadoutDialogue);
        }

        #endregion
    }
}
