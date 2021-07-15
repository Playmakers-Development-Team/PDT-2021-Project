using System.Collections;
using E7.Minefield;
using NUnit.Framework;
using Tests.Beacons;
using Tests.Constraints;
using Tests.Utilities;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    public class SimpleTests : BaseTest
    {
        [UnityTest]
        [Timeout(2000)]
        public IEnumerator BasicGameRun()
        {
            yield return PrepareAndActivateScene();
        }

        [UnityTest]
        [Timeout(2000)]
        public IEnumerator MovePlayerUnit()
        {
            yield return PrepareAndActivateScene();
            yield return Beacon.WaitUntil(UnitBeacons.Estelle, Any.ActingUnit);
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            // Wait for a while here so tester can see the move visualisation if need be
            yield return new WaitForSecondsRealtime(0.1f);
            yield return InputBeacon.ClickLeft(GridBeacons.A);
            yield return Beacon.WaitUntil(GridBeacons.A, Any.PlayerUnit);
        }

        [UnityTest] 
        [Timeout(2000)]
        public IEnumerator BasicEnemyMove()
        {
            yield return PrepareAndActivateScene();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
        }
    }
}
