using UnityEngine;

namespace Tests.Beacons
{
    public interface ITileBeacon : IScreenBeacon
    {
        Vector2Int Coordinate { get; }
        
        Vector3 SnappedPosition { get; }
    }
}
