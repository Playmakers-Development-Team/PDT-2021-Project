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
    // NOTE: Input beacons for clicking in world space, Beacon.Click for screen space/UI
    public class SimpleTests : BaseTest
    {
        public IEnumerator BasicSelections()
        {
            // TODO: Asserts to check that the correct unit is being displayed and selected
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            yield return new WaitForSeconds(0.5f);
            yield return InputBeacon.ClickLeft(UnitBeacons.EnemyA);
            yield return new WaitForSeconds(0.5f);
            yield return Beacon.ClickWhen(UITimelineBeacons.Pos1, Is.Clickable);
            yield return new WaitForSeconds(0.5f);
            yield return Beacon.ClickWhen(UITimelineBeacons.Pos2, Is.Clickable);
            yield return new WaitForSeconds(0.5f);
        }
        
        public IEnumerator MoveEstelle()
        {
            yield return Beacon.WaitUntil(UnitBeacons.Estelle, Any.ActingUnit);
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            // Wait for a while here so tester can see the move visualisation if need be
            yield return new WaitForSecondsRealtime(0.3f);
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
        public IEnumerator SelectionTest()
        {
            yield return PrepareAndActivateScene();
            yield return BasicSelections();
        }
        
        [UnityTest]
        [Timeout(2000), Order(2)]
        public IEnumerator MovePlayerUnit()
        {
            yield return PrepareAndActivateScene();
            yield return MoveEstelle();
        }

        [UnityTest] 
        [Timeout(2000), Order(3)]
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
        [Timeout(8000), Order(4)]
        public IEnumerator StandardTest()
        {
            yield return PrepareAndActivateScene();
            yield return BasicSelections();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
            yield return MoveEstelle();
            yield return Beacon.ClickWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return new WaitForSecondsRealtime(0.2f); // Waits so tester can confirm it was selected
            
            // Right click on unit or grid beacon but not both. Functionally the same but depends 
            // on how we want to do it
            //yield return InputBeacon.ClickRight(GridBeacons.B);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(9));
            yield return new WaitForSecondsRealtime(2.5f);
            Assert.Beacon(UnitBeacons.Estelle, Any.UnitEqualsHealth(7));
        }
    }
}
