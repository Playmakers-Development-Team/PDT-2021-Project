using System;
using E7.Minefield;
using Units;
using UnityEngine;

namespace Tests.Utilities
{
    public static class IUnitTestExtensions
    {
        public static bool HasBeacon<T>(this IUnit unit, T beacon, BeaconConstraint constraint) where T : Enum
        {
            if (unit is Component component)
            {
                var labelBeacon = component.GetComponent<ILabelBeacon>();
                return labelBeacon != null && labelBeacon.Label is T foundBeacon && Equals(foundBeacon, beacon);
            }

            return false;
        }
    }
}
