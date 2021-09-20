using System.Collections;
using E7.Minefield;
using NUnit.Framework;
using TenetStatuses;
using Tests.Beacons;
using Tests.Constraints;
using Tests.Utilities;
using Turn.Commands;
using Units.Commands;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.AutomatedTests
{
    [Category("Automated Testing")]
    public class TurnTests : BaseTest
    {
        protected override string Scene => "TurnTest";

        [UnityTest, Order(10)] 
        [Timeout(60000)]
        public IEnumerator KillUnitTurnOrder()
        {
            yield return PrepareAndActivateScene();
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Estelle, 
                UIBeacons.AbilityA, GridBeacons.A);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityB, GridBeacons.A);
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Estelle, 
                UIBeacons.AbilityA, GridBeacons.A);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityB, GridBeacons.A);
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Estelle, 
                UIBeacons.AbilityA, GridBeacons.A);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityC, GridBeacons.A);
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Estelle, 
                UIBeacons.AbilityA, GridBeacons.A);
            
            yield return InputTester.UnitUseAbility(UnitBeacons.Niles, 
                UIBeacons.AbilityC, GridBeacons.A);
            
            yield return TurnTester.WaitUnitTurn(UnitBeacons.Estelle);
            
            Assert.Beacon(UnitBeacons.Estelle, Any.UnitEqualsHealth(10));
        }
    }
}
