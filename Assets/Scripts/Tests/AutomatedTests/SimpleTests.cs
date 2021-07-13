using System.Collections;
using E7.Minefield;
using Grid;
using Managers;
using NUnit.Framework;
using Tests.Beacons;
using Turn;
using Units;
using Units.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;


namespace Tests.AutomatedTests
{
    [TestFixture]
    public class SimpleTests : SceneTest
    {
        private InputBeacon inputBeacon = new InputBeacon();

        protected override string Scene => "MainTest";

        public GridManager GridManager => ManagerLocator.Get<GridManager>();
        public TurnManager TurnManager => ManagerLocator.Get<TurnManager>();

        [UnityTest]
        public IEnumerator TestGameRun()
        {
            yield return ActivateScene();
        }
    }
}
