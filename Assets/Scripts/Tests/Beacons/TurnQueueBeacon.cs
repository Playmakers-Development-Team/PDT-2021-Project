using E7.Minefield;

namespace Tests.Beacons
{
    public enum TurnQueueBeacons
    {
        PlayerA, PlayerB, PlayerC, EnemyA, EnemyB, EnemyC, TurnManipulation 
    }
    
    public class TurnQueueBeacon : NavigationBeacon<TurnQueueBeacons> {}
}
