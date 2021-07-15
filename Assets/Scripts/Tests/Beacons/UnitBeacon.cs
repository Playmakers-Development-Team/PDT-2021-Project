using E7.Minefield;
using Tests.Beacons.Base;
using UnityEngine;

namespace Tests.Beacons
{
    public enum UnitBeacons
    {
        Estelle, Niles, Helena, EnemyA, EnemyB, EnemyC, EnemyD, PlayerA, PlayerB, PlayerC
    }

    public class UnitBeacon : TileBeacon<UnitBeacons> {}
}
