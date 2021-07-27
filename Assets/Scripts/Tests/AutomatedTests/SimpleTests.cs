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
    /// <summary>
    /// Simple tests to verify the game works. 
    /// </summary>
    // Goes top down, to ensure this, please have the order be ascending otherwise it will
    // go in alphabetical order
    public class SimpleTests : BaseTest
    {
        public IEnumerator MoveEstelle()
        {
            yield return Beacon.WaitUntil(UnitBeacons.Estelle, Any.ActingUnit);
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            // Wait for a while here so tester can see the move visualisation if need be
            yield return new WaitForSecondsRealtime(0.1f);
            yield return InputBeacon.ClickLeft(GridBeacons.A);
            yield return Beacon.WaitUntil(GridBeacons.A, Any.PlayerUnit);
        }
        
        [UnityTest]
        [Timeout(2000), Order(0)]
        public IEnumerator BasicGameRun()
        {
            yield return PrepareAndActivateScene();
        }

        [UnityTest]
        [Timeout(2000), Order(1)]
        public IEnumerator MovePlayerUnit()
        {
            yield return PrepareAndActivateScene();
            yield return MoveEstelle();
        }

        [UnityTest] 
        [Timeout(2000), Order(2)]
        public IEnumerator BasicEnemyMove()
        {
            yield return PrepareAndActivateScene();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
        }

        /// <summary>
        /// This is the standard test all tasks must pass
        /// </summary>
        /// <returns></returns>
        [UnityTest] 
        [Timeout(3000), Order(3)]
        public IEnumerator StandardTest()
        {
            yield return PrepareAndActivateScene();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
            yield return MoveEstelle();
            yield return Beacon.ClickWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return InputBeacon.ClickRight(GridBeacons.B);
            //yield return new WaitForSecondsRealtime(1f);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(9));
        }
    }
}
