using System;
using Commands;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Managers;
using Turn.Commands;
using UnityEngine;

namespace Game
{
    public class EndEncounterOnNoEnemies : MonoBehaviour
    {
        [SerializeField] private float delay = 2f;
        
        private CommandManager commandManager;
        
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        private void OnEnable()
        {
            commandManager.ListenCommand<NoRemainingEnemyUnitsCommand>(OnNoEnemyUnits);
        }

        private void OnDisable()
        {
            commandManager.UnlistenCommand<NoRemainingEnemyUnitsCommand>(OnNoEnemyUnits);
        }

        private async void OnNoEnemyUnits(NoRemainingEnemyUnitsCommand cmd)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            commandManager.ExecuteCommand(new EndEncounterCommand());
        }
    }
}
