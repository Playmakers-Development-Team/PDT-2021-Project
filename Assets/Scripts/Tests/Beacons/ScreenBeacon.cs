using System;
using E7.Minefield;
using UnityEngine;

namespace Tests.Beacons
{
    public class ScreenBeacon<T> : LabelBeacon<T>, IScreenBeacon where T : Enum
    {
        public virtual Vector3 WorldClickPosition => gameObject.transform.position;

        public virtual Vector2 ScreenClickPosition => Camera.main.WorldToScreenPoint(WorldClickPosition);
    }
}
