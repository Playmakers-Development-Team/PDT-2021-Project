using System.Collections;
using Commands;
using Cysharp.Threading.Tasks;
using E7.Minefield;
using Managers;
using NUnit.Framework;
using Tests.Beacons.Base;
using Turn.Commands;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    [Category("Test fixing")]
    public class TestsFixes : SceneTest
    {
        protected InputBeacon InputBeacon { get; } = new InputBeacon();
        protected CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        // The Default testing scene is MainTest
        protected override string Scene => "MainTest";
        
        [UnityTest]
        public IEnumerator RestoreInputs()
        {
            ManagerLocator.Initialize();
            UniTask task = CommandManager.WaitForCommand<TurnQueueCreatedCommand>();
            yield return ActivateScene();
            yield return new WaitUntil(() => task.Status.IsCompleted());
            InputBeacon.RestoreRegularDevices();
        }
    }
}
