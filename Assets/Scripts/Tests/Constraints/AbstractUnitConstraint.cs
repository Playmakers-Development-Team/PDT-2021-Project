using System.Linq;
using E7.Minefield;
using Grid;
using Managers;
using NUnit.Framework.Constraints;
using Tests.Beacons;
using Units;
using UnityEngine;

namespace Tests.Constraints
{
    public abstract class AbstractUnitConstraint : BeaconConstraint
    {
        protected virtual bool IsCorrectUnit(IUnit unit) => true;
                
        protected override ConstraintResult Assert()
        {
            bool foundCorrectUnit = false;
            
            if (FoundBeacon is ITileBeacon tileBeacon)
            {
                GridManager gridManager = ManagerLocator.Get<GridManager>();
                Vector2Int coordinate = tileBeacon.Coordinate;

                // This code here should really be an API we can use in the UnitManager or GridManager
                IUnit unit = gridManager.GetGridObjectsByCoordinate(coordinate)
                    .OfType<IUnit>().FirstOrDefault();

                // Check if we have found a unit
                if (unit != null && IsCorrectUnit(unit))
                {
                    foundCorrectUnit = true;
                }
            }

            return new ConstraintResult(this, FoundBeacon, foundCorrectUnit);
        }
    }
}
