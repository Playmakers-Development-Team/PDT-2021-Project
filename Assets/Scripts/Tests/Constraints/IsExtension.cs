using System;
using Units.Enemies;
using Units.Players;

namespace Tests.Constraints
{
    public partial class Is : NUnit.Framework.Is
    {
        public static UnitAtConstraint AnyUnit => new UnitAtConstraint();

        public static UnitAtConstraint AnyPlayerUnit => new UnitAtConstraint(typeof(PlayerUnit));

        public static UnitAtConstraint AnyEnemyUnit => new UnitAtConstraint(typeof(EnemyUnit));

        public static UnitTurnConstraint AnyActingUnit => new UnitTurnConstraint();

        public static UnitAtConstraint UnitInstanceOfType(Type withType) => 
            new UnitAtConstraint(withType);

        public static UnitAtConstraint UnitWithBeacon<T>(T beacon) where T : Enum =>
            new UnitAtConstraint<T>(beacon);
    }
}