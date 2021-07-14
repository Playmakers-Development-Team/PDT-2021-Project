using System;
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
    public class UnitAtConstraint : AbstractUnitConstraint
    {
        protected readonly Type withUnitType;

        public UnitAtConstraint(Type withType = null)
        {
            withUnitType = withType;
        }

        protected override bool IsCorrectUnit(IUnit unit) => 
            withUnitType == null || withUnitType.IsInstanceOfType(unit);
    }

    public class UnitAtConstraint<T> : UnitAtConstraint where T : Enum
    {
        private readonly T withBeacon;
        
        public UnitAtConstraint(T withBeacon, Type withType = null) : base(withType)
        {
            this.withBeacon = withBeacon;
        }

        protected override bool IsCorrectUnit(IUnit unit) => 
            base.IsCorrectUnit(unit) && Beacon.FindActive(withBeacon, out ILabelBeacon found);
    }
}
