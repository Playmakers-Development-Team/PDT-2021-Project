using System;
using Units.Enemies;
using Units.Players;

namespace Tests.Constraints
{
    public class Any
    {
        public static UnitAtConstraint Unit => new UnitAtConstraint();

        public static UnitAtConstraint PlayerUnit => new UnitAtConstraint(typeof(PlayerUnit));

        public static UnitAtConstraint EnemyUnit => new UnitAtConstraint(typeof(EnemyUnit));

        public static UnitTurnConstraint ActingUnit => new UnitTurnConstraint();

        public static UnitAtConstraint UnitInstanceOfType(Type withType) => 
            new UnitAtConstraint(withType);

        public static UnitAtConstraint UnitWithBeacon<T>(T beacon) where T : Enum =>
            new UnitAtConstraint<T>(beacon);
    }
}