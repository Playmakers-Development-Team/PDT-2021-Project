using System;
using System.Collections;
using E7.Minefield;
using Managers;
using NUnit.Framework;
using Tests.Beacons;
using Tests.Beacons.Base;
using Tests.Constraints;
using Turn;
using Turn.Commands;
using Units.Commands;
using UnityEngine;

namespace Tests.Utilities
{
    public class InputTester
    {
        private readonly InputBeacon inputBeacon;

        public InputTester(InputBeacon inputBeacon)
        {
            this.inputBeacon = inputBeacon;
        }

        public IEnumerator MoveUnitTo<Tu, Tt>(Tu unitBeacon, Tt targetBeacon) 
            where Tu : Enum
            where Tt : Enum
        {
            yield return TurnTester.WaitUnitTurn(unitBeacon);
            TestContext.WriteLine($"Test - Moving {unitBeacon} to target {targetBeacon}");
            yield return inputBeacon.ClickLeft(UIBeacons.Move);
            // Wait for a while here so tester can see the move visualisation if need be
            yield return new WaitForSecondsRealtime(0.1f);
            yield return inputBeacon.ClickLeft(targetBeacon);
            yield return Beacon.WaitUntil(targetBeacon, Any.UnitWithBeacon(unitBeacon));
        }

        public IEnumerator UnitUseAbility<Tu, Ta, Tt>(Tu unitBeacon, Ta abilityNavBeacon, Tt targetBeacon)
            where Tu : Enum
            where Ta : Enum
            where Tt : Enum
        {
            yield return TurnTester.WaitUnitTurn(unitBeacon);
            CommandWatcher endTurnWatcher = CommandWatcher.BeginWatching<EndTurnCommand>();
            TestContext.WriteLine($"Test - {unitBeacon}, using ability {abilityNavBeacon} to target {targetBeacon}");
            yield return inputBeacon.ClickLeftWhen(abilityNavBeacon, Is.Clickable);
            yield return inputBeacon.ClickRight(targetBeacon);
            yield return endTurnWatcher.WaitWithDefaultTimeout();
        }
    }
}
