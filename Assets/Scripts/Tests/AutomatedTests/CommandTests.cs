using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using E7.Minefield;
using NUnit.Framework;
using Tests.Beacons;
using Tests.Constraints;
using Tests.Samples;
using Tests.Utilities;
using UI.Commands;
using Units.Commands;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    /// <summary>
    /// Simple tests to verify the game works. 
    /// </summary>
    // Goes top down, to ensure this, please have the order be ascending otherwise it will
    // go in alphabetical order
    // NOTE: Input beacons for clicking in world space and screen space. Use Click() for world space
    // and ClickLeftWhen() for screen space.
    // Also Timeouts should be enough to complete the test + a reasonable extra amount of time to allow
    // for slower PCs to complete.
    [Category("Automated Testing")]
    public class CommandTests : BaseTest
    {
        [UnityTest] 
        [Timeout(8000)] 
        [Order(0)]
        public IEnumerator AwaitFinishCommands() => UniTask.ToCoroutine(async () =>
        {
            await PrepareAndActivateScene().ToUniTask();
            UniTask waitFinish = CommandManager.ExecuteAndAwaitFinish(new SomeOperationCommand());
            Assert.False(waitFinish.Status.IsCompleted(), "Awaited command is not finished, but it looks like it is!");
            CommandManager.ExecuteCommand(new SomeOperationFinishedCommand());
            await waitFinish;
        });
    }
}
