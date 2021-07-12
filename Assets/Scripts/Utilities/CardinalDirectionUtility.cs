using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class CardinalDirectionUtility
    {
        public static CardinalDirection From(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 vector = (toPos - fromPos).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, vector);

            if (angle >= 45 && angle < 135)
            {
                return CardinalDirection.West;
            }

            if (angle < -45 && angle >= -135)
            {
                return CardinalDirection.East;
            }

            if (angle >= -45 && angle < 45)
            {
                return CardinalDirection.North;
            }

            return CardinalDirection.South;
        }

        public static Vector2Int RotateVector2Int(Vector2Int point,
                                                       CardinalDirection fromDirection,
                                                       CardinalDirection toDirection)
        {
            float angle = Vector2.SignedAngle(fromDirection.ToVector2(), toDirection.ToVector2());

            if (angle >= 45 && angle < 135)
            {
                return new Vector2Int(-point.y, point.x);
            }

            if (angle < -45 && angle >= -135)
            {
                return new Vector2Int(point.y, -point.x);
            }

            if (angle >= -45 && angle < 45)
            {
                return point;
            }

            return new Vector2Int(-point.x, -point.y);
        }

        public static Vector2 RotateVector2(Vector2 point, CardinalDirection fromDirection,
                                                 CardinalDirection toDirection)
        {
            float angle = Vector2.SignedAngle(fromDirection.ToVector2(), toDirection.ToVector2());

            if (angle >= 45 && angle < 135)
            {
                return new Vector2(-point.y, point.x);
            }

            if (angle < -45 && angle >= -135)
            {
                return new Vector2(point.y, -point.x);
            }

            if (angle >= -45 && angle < 45)
            {
                return point;
            }

            return new Vector2(-point.x, -point.y);
        }

        public static CardinalDirectionMask GetMaskFrom(params CardinalDirection[] directions) =>
            GetMaskFrom(directions.AsEnumerable());

        public static CardinalDirectionMask GetMaskFrom(IEnumerable<CardinalDirection> directions)
        {
            CardinalDirectionMask mask = CardinalDirectionMask.None;

            foreach (CardinalDirection direction in directions)
            {
                mask |= direction.ToMask();
            }

            return mask;
        }
    }
}
