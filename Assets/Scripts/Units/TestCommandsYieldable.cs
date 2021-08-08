using System;
using System.Collections;
using Commands;
using Managers;
using Units.Commands;
using Units.Players;
using UnityEngine;

namespace Units
{
    /// <summary>
    /// An example script used to use commands with await and coroutines. This script outputs
    /// spawn command in debug log.
    /// </summary>
    // TODO Move this to the Tests assembly/folder once it's there
    public class TestCommandsYieldable : MonoBehaviour
    {
        private CommandManager commandManager;
        
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            
            // We're testing specific listening and un-listening functions from the command manager
            commandManager.ListenCommand(typeof(UnitsReadyCommand<PlayerUnitData>), SomeFunction);
            commandManager.UnlistenCommand(typeof(UnitsReadyCommand<PlayerUnitData>), SomeFunction);

            // We're testing the yieldable/awaitable functions
            SomeAsyncFunction();
            StartCoroutine(SomeCoroutine());
        }

        private void SomeFunction()
        {
            Debug.LogWarning("Player units ready, though this should not be called!");
        }

        private async void SomeAsyncFunction()
        {
            SpawnedUnitCommand spawnedUnitCommand = await commandManager.WaitForCommand<SpawnedUnitCommand>();
            Debug.Log($"Testing command async... spawned unit {spawnedUnitCommand.Unit}!");
        }

        private IEnumerator SomeCoroutine()
        {
            yield return commandManager.WaitForCommandYield<SpawnedUnitCommand>();
            Debug.Log("Testing command corountine... unit spawned!");
        }
    }
}
