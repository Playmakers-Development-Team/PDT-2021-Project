using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public enum QuadDirection
    {
        North,
        East,
        South,
        West
    }

    public static class QuadDirectionExtensions
    {
        public static QuadDirection Opposite(this QuadDirection direction) =>
            (QuadDirection) (((int) direction + 2) % 4);

        public static QuadDirection RotateClockwise(this QuadDirection direction) =>
            (QuadDirection) (((int) direction + 1) % 4);

        public static QuadDirection RotateAntiClockwise(this QuadDirection direction)
        {
            int directionIndex = (int) direction - 1;
            return (QuadDirection) (directionIndex < 0 ? 3 : directionIndex % 4);
        }

        public static Vector2Int ToVector2Int(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.East:
                    return Vector2Int.right;
                case QuadDirection.South:
                    return Vector2Int.down;
                case QuadDirection.West:
                    return Vector2Int.left;
                default:
                    return Vector2Int.up;
            }
        }

        public static Vector2 ToVector2(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.East:
                    return Vector2.right;
                case QuadDirection.South:
                    return Vector2.down;
                case QuadDirection.West:
                    return Vector2.left;
                default:
                    return Vector2.up;
            }
        }

        public static QuadDirectionMask ToMask(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.East:
                    return QuadDirectionMask.East;
                case QuadDirection.South:
                    return QuadDirectionMask.South;
                case QuadDirection.West:
                    return QuadDirectionMask.West;
                default:
                    return QuadDirectionMask.North;
            }
        }

        public static Direction ToDirection(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.East:
                    return Direction.East;
                case QuadDirection.South:
                    return Direction.South;
                case QuadDirection.West:
                    return Direction.West;
                default:
                    return Direction.North;
            }
        }

        public static bool IsVertical(this QuadDirection direction) =>
            direction == QuadDirection.North || direction == QuadDirection.South;

        public static bool IsHorizontal(this QuadDirection direction) =>
            direction == QuadDirection.East || direction == QuadDirection.West;
    }

    [Flags]
    public enum QuadDirectionMask
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
    }

    public static class QuadDirectionMaskExtensions
    {
        public static bool Contains(this QuadDirectionMask mask, QuadDirection direction) =>
            (mask & direction.ToMask()) != 0;

        public static IEnumerable<QuadDirectionMask> Split(this QuadDirectionMask mask)
        {
            List<QuadDirectionMask> enumerable = new List<QuadDirectionMask>(4);

            if ((mask & QuadDirectionMask.North) != 0)
            {
                enumerable.Add(QuadDirectionMask.North);
            }

            if ((mask & QuadDirectionMask.East) != 0)
            {
                enumerable.Add(QuadDirectionMask.East);
            }

            if ((mask & QuadDirectionMask.West) != 0)
            {
                enumerable.Add(QuadDirectionMask.West);
            }

            if ((mask & QuadDirectionMask.South) != 0)
            {
                enumerable.Add(QuadDirectionMask.South);
            }

            return enumerable;
        }

        public static IEnumerable<QuadDirection> GetDirections(this QuadDirectionMask mask)
        {
            List<QuadDirection> enumerable = new List<QuadDirection>(4);

            if ((mask & QuadDirectionMask.North) != 0)
            {
                enumerable.Add(QuadDirection.North);
            }

            if ((mask & QuadDirectionMask.East) != 0)
            {
                enumerable.Add(QuadDirection.East);
            }

            if ((mask & QuadDirectionMask.West) != 0)
            {
                enumerable.Add(QuadDirection.West);
            }

            if ((mask & QuadDirectionMask.South) != 0)
            {
                enumerable.Add(QuadDirection.South);
            }

            return enumerable;
        }

        public static bool HasVertical(this QuadDirectionMask mask) =>
            (mask & QuadDirectionMask.North) != 0 || (mask & QuadDirectionMask.South) != 0;

        public static bool HasHorizontal(this QuadDirectionMask mask) =>
            (mask & QuadDirectionMask.East) != 0 || (mask & QuadDirectionMask.West) != 0;
    }

    public static class QuadDirectionUtil
    {
        public static QuadDirection From(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 vector = (toPos - fromPos).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, vector);

            if (angle >= 45 && angle < 135)
            {
                return QuadDirection.West;
            }

            if (angle < -45 && angle >= -135)
            {
                return QuadDirection.East;
            }

            if (angle >= -45 && angle < 45)
            {
                return QuadDirection.North;
            }

            return QuadDirection.South;
        }

        public static QuadDirectionMask GetMaskFrom(params QuadDirection[] directions) =>
            GetMaskFrom(directions.AsEnumerable());

        public static QuadDirectionMask GetMaskFrom(IEnumerable<QuadDirection> directions)
        {
            QuadDirectionMask mask = QuadDirectionMask.None;

            foreach (QuadDirection direction in directions)
            {
                mask |= direction.ToMask();
            }

            return mask;
        }
    }
}
