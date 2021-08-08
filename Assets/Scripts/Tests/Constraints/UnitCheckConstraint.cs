using System;
using Managers;
using Turn;
using Units;

namespace Tests.Constraints
{
    public class UnitCheckConstraint : AbstractUnitConstraint
    {
        private Predicate<IUnit> predicate;
        
        public override string Description { get; protected set; }

        protected override bool IsCorrectUnit(IUnit unit) => predicate(unit);

        public UnitCheckConstraint(Predicate<IUnit> predicate, string description)
        {
            this.predicate = predicate;
            Description = description;
        }

    }
}
