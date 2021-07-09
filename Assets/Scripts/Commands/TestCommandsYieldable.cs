using System;
using System.Collections;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Commands
{
    /// <summary>
    /// An example script used to use commands with await and coroutines. This script outputs
    /// spawn command in debug log.
    /// </summary>
    
    // TODO Delete this file and the CommandYieldTest Scene after merge to dev.
    public class TestCommandsYieldable : MonoBehaviour
    {
        private CommandManager commandManager;
        
        private void Awake()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            
            SomeAsyncFunction();
            StartCoroutine(SomeCoroutine());
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
