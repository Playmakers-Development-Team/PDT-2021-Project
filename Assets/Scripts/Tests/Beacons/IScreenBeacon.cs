using UnityEngine;

namespace Tests.Beacons
{
    public interface IScreenBeacon
    {
        GameObject gameObject { get; }
        
        Vector3 WorldClickPosition { get; }

        Vector2 ScreenClickPosition { get; }
    }
}
