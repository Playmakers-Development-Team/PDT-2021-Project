using E7.Minefield;

namespace Tests.Beacons
{
    public enum UnitNavBeacons
    {
        PlayerA, PlayerB, PlayerC, EnemyA, EnemyB, EnemyC 
    }
    
    public class UnitNavBeacon : NavigationBeacon<UnitNavBeacons> {}
}
