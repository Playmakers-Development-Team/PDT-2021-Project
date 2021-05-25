using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public enum OrdinalDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public static class OrdinalDirectionExtensions
    {
        public static OrdinalDirection Opposite(this OrdinalDirection direction) =>
            (OrdinalDirection) (((int) direction + 4) % 8);

        public static OrdinalDirection RotateClockwise(this OrdinalDirection direction) =>
            (OrdinalDirection) (((int) direction + 1) % 8);

        public static OrdinalDirection RotateAntiClockwise(this OrdinalDirection direction)
        {
            int directionIndex = (int) direction - 1;
            return (OrdinalDirection) (directionIndex < 0 ? 7 : directionIndex % 8);
        }

        public static Vector2Int ToVector2Int(this OrdinalDirection direction)
        {
            switch (direction)
            {
                case OrdinalDirection.North:
                    return Vector2Int.up;
                case OrdinalDirection.NorthEast:
                    return Vector2Int.up + Vector2Int.right;
                case OrdinalDirection.East:
                    return Vector2Int.right;
                case OrdinalDirection.SouthEast:
                    return Vector2Int.down + Vector2Int.right;
                case OrdinalDirection.South:
                    return Vector2Int.down;
                case OrdinalDirection.SouthWest:
                    return Vector2Int.down + Vector2Int.left;
                case OrdinalDirection.West:
                    return Vector2Int.left;
                case OrdinalDirection.NorthWest:
                    return Vector2Int.up + Vector2Int.left;
            }

            throw new Exception($"{nameof(OrdinalDirection)} {direction} is not handled");
        }

        public static Vector2 ToVector2(this OrdinalDirection direction)
        {
            switch (direction)
            {
                case OrdinalDirection.North:
                    return Vector2.up;
                case OrdinalDirection.NorthEast:
                    return Vector2.up + Vector2.right;
                case OrdinalDirection.East:
                    return Vector2.right;
                case OrdinalDirection.SouthEast:
                    return Vector2.down + Vector2.right;
                case OrdinalDirection.South:
                    return Vector2.down;
                case OrdinalDirection.SouthWest:
                    return Vector2.down + Vector2.left;
                case OrdinalDirection.West:
                    return Vector2.left;
                case OrdinalDirection.NorthWest:
                    return Vector2.up + Vector2.left;
            }

            throw new Exception($"{nameof(OrdinalDirection)} {direction} is not handled");
        }

        public static OrdinalDirectionMask ToMask(this OrdinalDirection direction)
        {
            switch (direction)
            {
                case OrdinalDirection.North:
                    return OrdinalDirectionMask.North;
                case OrdinalDirection.NorthEast:
                    return OrdinalDirectionMask.NorthEast;
                case OrdinalDirection.East:
                    return OrdinalDirectionMask.East;
                case OrdinalDirection.SouthEast:
                    return OrdinalDirectionMask.SouthEast;
                case OrdinalDirection.South:
                    return OrdinalDirectionMask.South;
                case OrdinalDirection.SouthWest:
                    return OrdinalDirectionMask.SouthWest;
                case OrdinalDirection.West:
                    return OrdinalDirectionMask.West;
                case OrdinalDirection.NorthWest:
                    return OrdinalDirectionMask.NorthWest;
            }

            throw new Exception($"{nameof(OrdinalDirection)} {direction} is not handled");
        }

        public static CardinalDirection ToCardinalDirection(this OrdinalDirection direction)
        {
            OrdinalDirection newDirection = direction;

            if (direction.IsDiagonal())
            {
                newDirection = newDirection.RotateClockwise();
            }

            switch (newDirection)
            {
                case OrdinalDirection.East:
                    return CardinalDirection.East;
                case OrdinalDirection.South:
                    return CardinalDirection.South;
                case OrdinalDirection.West:
                    return CardinalDirection.West;
                default:
                    return CardinalDirection.North;
            }
        }

        public static bool IsHorizontal(this OrdinalDirection direction) =>
            direction == OrdinalDirection.East || direction == OrdinalDirection.West;

        public static bool IsVertical(this OrdinalDirection direction) =>
            direction == OrdinalDirection.North || direction == OrdinalDirection.South;

        public static bool IsDiagonal(this OrdinalDirection direction) =>
            direction == OrdinalDirection.NorthEast || direction == OrdinalDirection.NorthWest
                                             || direction == OrdinalDirection.SouthWest
                                             || direction == OrdinalDirection.SouthEast;
    }

    [Flags]
    public enum OrdinalDirectionMask
    {
        None = 0,
        North = 1,
        NorthEast = 2,
        East = 4,
        SouthEast = 8,
        South = 16,
        SouthWest = 32,
        West = 64,
        NorthWest = 128
    }

    public static class OrdinalDirectionMaskExtensions
    {
        public static bool Contains(this OrdinalDirectionMask mask, OrdinalDirection direction) =>
            (mask & direction.ToMask()) != 0;

        public static IEnumerable<OrdinalDirectionMask> Split(this OrdinalDirectionMask mask)
        {
            List<OrdinalDirectionMask> enumerable = new List<OrdinalDirectionMask>(4);

            if ((mask & OrdinalDirectionMask.North) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.North);
            }

            if ((mask & OrdinalDirectionMask.NorthEast) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.NorthEast);
            }

            if ((mask & OrdinalDirectionMask.East) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.East);
            }

            if ((mask & OrdinalDirectionMask.SouthEast) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.SouthEast);
            }

            if ((mask & OrdinalDirectionMask.South) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.South);
            }

            if ((mask & OrdinalDirectionMask.SouthWest) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.SouthWest);
            }

            if ((mask & OrdinalDirectionMask.West) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.West);
            }

            if ((mask & OrdinalDirectionMask.NorthWest) != 0)
            {
                enumerable.Add(OrdinalDirectionMask.NorthWest);
            }

            return enumerable;
        }

        public static IEnumerable<OrdinalDirection> GetDirections(this OrdinalDirectionMask mask)
        {
            List<OrdinalDirection> enumerable = new List<OrdinalDirection>(4);

            if ((mask & OrdinalDirectionMask.North) != 0)
            {
                enumerable.Add(OrdinalDirection.North);
            }

            if ((mask & OrdinalDirectionMask.NorthEast) != 0)
            {
                enumerable.Add(OrdinalDirection.NorthEast);
            }

            if ((mask & OrdinalDirectionMask.East) != 0)
            {
                enumerable.Add(OrdinalDirection.East);
            }

            if ((mask & OrdinalDirectionMask.SouthEast) != 0)
            {
                enumerable.Add(OrdinalDirection.SouthEast);
            }

            if ((mask & OrdinalDirectionMask.South) != 0)
            {
                enumerable.Add(OrdinalDirection.South);
            }

            if ((mask & OrdinalDirectionMask.SouthWest) != 0)
            {
                enumerable.Add(OrdinalDirection.SouthWest);
            }

            if ((mask & OrdinalDirectionMask.West) != 0)
            {
                enumerable.Add(OrdinalDirection.West);
            }

            if ((mask & OrdinalDirectionMask.NorthWest) != 0)
            {
                enumerable.Add(OrdinalDirection.NorthWest);
            }

            return enumerable;
        }

        public static bool HasVertical(this OrdinalDirectionMask mask) =>
            (mask & OrdinalDirectionMask.North) != 0 || (mask & OrdinalDirectionMask.South) != 0;

        public static bool HasHorizontal(this OrdinalDirectionMask mask) =>
            (mask & OrdinalDirectionMask.East) != 0 || (mask & OrdinalDirectionMask.West) != 0;

        public static bool HasDiagonal(this OrdinalDirectionMask mask) =>
            (mask & OrdinalDirectionMask.NorthEast) != 0 || (mask & OrdinalDirectionMask.NorthWest) != 0
                                                  || (mask & OrdinalDirectionMask.SouthEast) != 0
                                                  || (mask & OrdinalDirectionMask.SouthWest) != 0;
    }

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
