using System.Collections;
using E7.Minefield;
using NUnit.Framework;
using Tests.Beacons;
using Tests.Constraints;
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
    public class SimpleTests : BaseTest
    {
        #region Basic Test Cases

        private IEnumerator BasicSelections()
        {
            var estelleSelectedWatch = CommandWatcher
                .BeginWatching<UIUnitSelectedCommand>(cmd => cmd.Unit.HasBeacon(UnitBeacons.Estelle, Is.Active));
            var enemySelectedWatch = CommandWatcher
                .BeginWatching<UIUnitSelectedCommand>(cmd => cmd.Unit.HasBeacon(UnitBeacons.EnemyA, Is.Active));
            
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            yield return new WaitForSeconds(0.5f);
            yield return InputBeacon.ClickLeft(UnitBeacons.EnemyA);
            yield return new WaitForSeconds(0.5f);
            
            estelleSelectedWatch.Assert("Can't select Estelle from mouse click");
            enemySelectedWatch.Assert("Can't select Enemy from mouse click");
            
            estelleSelectedWatch.Rewatch();
            enemySelectedWatch.Rewatch();
            
            yield return InputBeacon.ClickLeftWhen(UITimelineBeacons.Pos1, Is.Clickable);
            yield return new WaitForSeconds(0.5f);
            yield return InputBeacon.ClickLeftWhen(UITimelineBeacons.Pos2, Is.Clickable);
            yield return new WaitForSeconds(0.5f);
            
            estelleSelectedWatch.Assert("Can't select Estelle from turn queue");
            enemySelectedWatch.Assert("Can't select Enemy from turn queue");
        }

        private IEnumerator MoveEstelle()
        {
            yield return Beacon.WaitUntil(UnitBeacons.Estelle, Any.ActingUnit);
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            // Wait for a while here so tester can see the move visualisation if need be
            yield return new WaitForSecondsRealtime(0.3f);
            yield return InputBeacon.ClickLeft(GridBeacons.A);
            yield return Beacon.WaitUntil(GridBeacons.A, Any.PlayerUnit);
        }

        #endregion

        #region Unity Tests

        [UnityTest]
        [Timeout(2000)]
        [Order(0)]
        public IEnumerator BasicGameRun()
        {
            yield return PrepareAndActivateScene();
        }

        [UnityTest]
        [Timeout(5000)]
        [Order(1)]
        public IEnumerator SelectionTest()
        {
            yield return PrepareAndActivateScene();
            for (int i = 0; i < 10; i++)
            {
                yield return BasicSelections();
            }
        }
        
        [UnityTest]
        [Timeout(2000)]
        [Order(2)]
        public IEnumerator MovePlayerUnit()
        {
            yield return PrepareAndActivateScene();
            yield return MoveEstelle();
        }

        [UnityTest] 
        [Timeout(2000)]
        [Order(3)]
        public IEnumerator BasicEnemyMove()
        {
            yield return PrepareAndActivateScene();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
        }

        [UnityTest] 
        [Timeout(2000)]
        [Order(4)]
        public IEnumerator TurnTest()
        {
            yield return PrepareAndActivateScene();
            yield return InputBeacon.ClickLeftWhen(UIBeacons.EndTurn, Is.Clickable);
            yield return TurnTester.WaitPlayerTurn();
        }
        
        [UnityTest] 
        [Timeout(5000)]
        [Order(5)]
        public IEnumerator KillingEnemyUnits()
        {
            yield return PrepareAndActivateScene();
            yield return InputBeacon.ClickLeftWhen(UIBeacons.EndTurn, Is.Clickable);
            yield return TurnTester.WaitPlayerTurn();
            yield return MoveEstelle();
            
            var enemyDeathWatcher = CommandWatcher
                .BeginWatching<KilledUnitCommand>(cmd => cmd.Unit.HasBeacon(UnitBeacons.EnemyA, Is.Active));
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityB, Is.Clickable);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);

            //yield return enemyDeathWatcher.Wait();
            yield return enemyDeathWatcher.Assert("Failed");
            
            yield return new WaitForSecondsRealtime(0.2f);
        }
        
        [UnityTest] 
        [Timeout(5000)]
        [Order(5)]
        public IEnumerator KillingPlayerUnits()
        {
            yield return PrepareAndActivateScene();
            
            var unitDeathWatcher = CommandWatcher
                .BeginWatching<KilledUnitCommand>(cmd => cmd.Unit.HasBeacon(UnitBeacons.Estelle, Is.Active));
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityC, Is.Clickable);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return InputBeacon.ClickRight(UnitBeacons.Estelle);

            yield return unitDeathWatcher.Wait();
            
            yield return new WaitForSecondsRealtime(0.8f);
        }

        /// <summary>
        /// This is the standard test all tasks must pass
        /// </summary>
        /// <returns></returns>
        [UnityTest] 
        [Timeout(8000)]
        [Order(999)]
        public IEnumerator StandardTest()
        {
            yield return PrepareAndActivateScene();
            yield return BasicSelections();
            CommandTester.EndCurrentUnitTurn();
            yield return Beacon.WaitUntil(GridBeacons.B, Any.EnemyUnit);
            yield return TurnTester.WaitPlayerTurn();
            yield return MoveEstelle();
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return new WaitForSecondsRealtime(0.2f); // Waits so tester can confirm it was selected
            
            // Right click on unit or grid beacon but not both. Functionally the same but depends 
            // on how we want to do it
            //yield return InputBeacon.ClickRight(GridBeacons.B);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(9));
            yield return CommandManager.WaitForCommandYield<StatChangedCommand>(cmd => 
                cmd.Unit.HasBeacon(UnitBeacons.Estelle, Is.Active));
            Assert.Beacon(UnitBeacons.Estelle, Any.UnitEqualsHealth(9));
            yield return CommandManager.WaitForCommandYield<StatChangedCommand>(cmd => 
                cmd.Unit.HasBeacon(UnitBeacons.EnemyA, Is.Active));
            yield return new WaitForSecondsRealtime(1.0f);
        }

        #endregion
        
    }
}
