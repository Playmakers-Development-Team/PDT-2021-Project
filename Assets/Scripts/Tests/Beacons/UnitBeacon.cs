using E7.Minefield;
using UnityEngine;

namespace Tests.Beacons
{
    public enum UnitBeacons
    {
        PlayerA, PlayerB, PlayerC, EnemyA, EnemyB, EnemyC 
    }
    
    public class UnitBeacon : LabelBeacon<UnitBeacons> {}
}
