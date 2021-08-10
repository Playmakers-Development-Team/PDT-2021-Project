using System.Collections;
using E7.Minefield;
using NUnit.Framework;
using TenetStatuses;
using Tests.Beacons;
using Tests.Constraints;
using Tests.Utilities;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    [Category("Automated Testing")]
    public class AbilityTests : BaseTest
    {
        protected override string Scene => "AbilityTest";

        [UnityTest, Order(0)]
        [Timeout(8000)]
        public IEnumerator ProvidingTenets()
        {
            TenetStatus expectedEnemyTenets = new TenetStatus(TenetType.Passion, 3);
            TenetStatus expectedEstelleTenets = new TenetStatus(TenetType.Pride, 1);
            
            yield return PrepareAndActivateScene();
            yield return InputBeacon.ClickLeft(UnitBeacons.Estelle);
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);

            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(expectedEstelleTenets));
            Assert.Beacon(UnitBeacons.EnemyA, Any.EqualsTenets(expectedEnemyTenets));
        }

        [UnityTest, Order(1)] 
        [Timeout(80000)]
        public IEnumerator AbilityCosts()
        {
            yield return PrepareAndActivateScene();
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityB, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(10));
            yield return TurnTester.NextTurnUntilUnit(UnitBeacons.Estelle);
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityA, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);
            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(10));
            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(new TenetStatus(TenetType.Pride, 1)));
            yield return TurnTester.NextTurnUntilUnit(UnitBeacons.Estelle);
            
            yield return InputBeacon.ClickLeftWhen(UIBeacons.AbilityB, Is.Clickable);
            yield return InputBeacon.ClickRight(UnitBeacons.EnemyA);

            Assert.Beacon(UnitBeacons.EnemyA, Any.UnitEqualsHealth(7));
            Assert.Beacon(UnitBeacons.Estelle, Any.EqualsTenets(new TenetStatus(TenetType.Pride, 0)));
            
            yield return DelayForViewing();
        }
    }
}
