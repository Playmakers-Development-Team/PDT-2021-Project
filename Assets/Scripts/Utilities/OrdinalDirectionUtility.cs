using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class OrdinalDirectionUtility
    {
        public static OrdinalDirection From(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 vector = (toPos - fromPos).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, vector);

            if (angle >= 22.5f && angle < 67.5f)
            {
                return OrdinalDirection.NorthWest;
            }

            if (angle >= 67.5f && angle < 112.5f)
            {
                return OrdinalDirection.West;
            }

            if (angle >= 112.5f && angle < 157.5f)
            {
                return OrdinalDirection.SouthWest;
            }

            if (angle < -22.5f && angle >= -67.5f)
            {
                return OrdinalDirection.NorthEast;
            }

            if (angle < -67.5f && angle >= -112.5f)
            {
                return OrdinalDirection.East;
            }

            if (angle < -112.5f && angle >= -157.5f)
            {
                return OrdinalDirection.SouthEast;
            }

            if (angle >= -22.5f && angle < 22.5f)
            {
                return OrdinalDirection.North;
            }

            return OrdinalDirection.South;
        }

        public static OrdinalDirectionMask GetMaskFrom(params OrdinalDirection[] directions) => 
            GetMaskFrom(directions.AsEnumerable());

        public static OrdinalDirectionMask GetMaskFrom(IEnumerable<OrdinalDirection> directions)
        {
            OrdinalDirectionMask mask = OrdinalDirectionMask.None;

            foreach (OrdinalDirection direction in directions)
            {
                mask |= direction.ToMask();
            }

            return mask;
        }
    }
}
