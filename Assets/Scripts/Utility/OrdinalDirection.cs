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
            direction == OrdinalDirection.NorthEast ||
            direction == OrdinalDirection.NorthWest ||
            direction == OrdinalDirection.SouthWest ||
            direction == OrdinalDirection.SouthEast;
    }
}
