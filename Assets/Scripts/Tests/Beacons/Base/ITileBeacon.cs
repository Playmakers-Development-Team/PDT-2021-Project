using UnityEngine;

namespace Tests.Beacons.Base
{
    public interface ITileBeacon : IScreenBeacon
    {
        Vector2Int Coordinate { get; }
        
        Vector3 SnappedPosition { get; }
    }
}
