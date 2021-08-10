using System;
using System.Collections;
using Commands;
using E7.Minefield;
using Managers;
using NUnit.Framework;
using Tests.Beacons;
using Turn;
using Turn.Commands;
using Units;
using Units.Enemies;
using Units.Players;
using UnityEngine;

namespace Tests.Utilities
{
    public static class TurnTester
    {
        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        private static TurnManager TurnManager => ManagerLocator.Get<TurnManager>();

        public static IEnumerator WaitPlayerTurn()
        {
            yield return new WaitUntil(() => TurnManager.ActingUnit is PlayerUnit);
        }
        
        public static IEnumerator WaitEnemyTurn()
        {
            yield return new WaitUntil(() => TurnManager.ActingUnit is EnemyUnit);
        }

        public static IEnumerator WaitUnitTurn<T>(T beacon) where T : Enum
        {
            if (Beacon.FindActive(beacon, out ILabelBeacon foundBeacon)
                && foundBeacon is UnitBeacon unitBeacon)
            {
                IUnit unit = unitBeacon.GetComponent<IUnit>();

                if (unit == null)
                    throw new ArgumentException($"Unit beacon {beacon} is not attached to an IUnit!");
                
                TestContext.WriteLine($"Test - Waiting until turn of {beacon}");
                
                while (TurnManager.ActingUnit != unit)
                {
                    CommandWatcher commandWatcher = CommandWatcher.BeginWatching<StartTurnCommand>();
                    
                    if (TurnManager.ActingUnit is PlayerUnit)
                        yield return Beacon.ClickWhen(UIBeacons.EndTurn, Is.Clickable);
                    else
                        yield return CommandManager.WaitForCommandYield<EndTurnCommand>();

                    yield return commandWatcher.WaitWithDefaultTimeout();
                }
                
                // Wait a frame so that we do stuff after the start turn command
                yield return null;
            }
            else
            {
                throw new ArgumentException($"Cannot find beacon {beacon} as a Unit Beacon");
            }
        }

        public static IEnumerator NextTurnUntilUnit<T>(T beacon) where T : Enum
        {
            CommandWatcher commandWatcher = CommandWatcher.BeginWatching<StartTurnCommand>();
            yield return Beacon.ClickWhen(UIBeacons.EndTurn, Is.Clickable);
            yield return commandWatcher.WaitWithDefaultTimeout();
            yield return WaitUnitTurn(beacon);
        }

        public static void DoUnit<T>(T beacon, Action<IUnit> action) where T : Enum
        {
            if (Beacon.FindActive(beacon, out ILabelBeacon foundBeacon)
                && foundBeacon is UnitBeacon unitBeacon)
            {
                IUnit unit = unitBeacon.GetComponent<IUnit>();
                
                if (unit == null)
                    throw new ArgumentException($"Unit beacon {beacon} is not attached to an IUnit!");

                action(unit);
            }
        }
    }
}
