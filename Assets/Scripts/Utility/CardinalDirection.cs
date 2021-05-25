using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }

    public static class CardinalDirectionExtensions
    {
        public static CardinalDirection Opposite(this CardinalDirection direction) =>
            (CardinalDirection) (((int) direction + 2) % 4);

        public static CardinalDirection RotateClockwise(this CardinalDirection direction) =>
            (CardinalDirection) (((int) direction + 1) % 4);

        public static CardinalDirection RotateAntiClockwise(this CardinalDirection direction)
        {
            int directionIndex = (int) direction - 1;
            return (CardinalDirection) (directionIndex < 0 ? 3 : directionIndex % 4);
        }

        public static Vector2Int ToVector2Int(this CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.East:
                    return Vector2Int.right;
                case CardinalDirection.South:
                    return Vector2Int.down;
                case CardinalDirection.West:
                    return Vector2Int.left;
                case CardinalDirection.North:
                    return Vector2Int.up;
            }

            throw new Exception($"{nameof(CardinalDirection)} {direction} is not handled");
        }

        public static Vector2 ToVector2(this CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.East:
                    return Vector2.right;
                case CardinalDirection.South:
                    return Vector2.down;
                case CardinalDirection.West:
                    return Vector2.left;
                case CardinalDirection.North:
                    return Vector2.up;
            }

            throw new Exception($"{nameof(CardinalDirection)} {direction} is not handled");
        }

        public static CardinalDirectionMask ToMask(this CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.East:
                    return CardinalDirectionMask.East;
                case CardinalDirection.South:
                    return CardinalDirectionMask.South;
                case CardinalDirection.West:
                    return CardinalDirectionMask.West;
                case CardinalDirection.North:
                    return CardinalDirectionMask.North;
            }

            throw new Exception($"{nameof(CardinalDirection)} {direction} is not handled");
        }

        public static OrdinalDirection ToOrdinalDirection(this CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.East:
                    return OrdinalDirection.East;
                case CardinalDirection.South:
                    return OrdinalDirection.South;
                case CardinalDirection.West:
                    return OrdinalDirection.West;
                case CardinalDirection.North:
                    return OrdinalDirection.North;
            }

            throw new Exception($"{nameof(CardinalDirection)} {direction} is not handled");
        }

        public static bool IsVertical(this CardinalDirection direction) =>
            direction == CardinalDirection.North || direction == CardinalDirection.South;

        public static bool IsHorizontal(this CardinalDirection direction) =>
            direction == CardinalDirection.East || direction == CardinalDirection.West;
    }

    [Flags]
    public enum CardinalDirectionMask
    {
        None = 0,
        North = 1,
        East = 2,
        South = 4,
        West = 8,
    }

    public static class CardinalDirectionMaskExtensions
    {
        public static bool Contains(this CardinalDirectionMask mask, CardinalDirection direction) =>
            (mask & direction.ToMask()) != 0;

        public static IEnumerable<CardinalDirectionMask> Split(this CardinalDirectionMask mask)
        {
            List<CardinalDirectionMask> enumerable = new List<CardinalDirectionMask>(4);

            if ((mask & CardinalDirectionMask.North) != 0)
            {
                enumerable.Add(CardinalDirectionMask.North);
            }

            if ((mask & CardinalDirectionMask.East) != 0)
            {
                enumerable.Add(CardinalDirectionMask.East);
            }

            if ((mask & CardinalDirectionMask.West) != 0)
            {
                enumerable.Add(CardinalDirectionMask.West);
            }

            if ((mask & CardinalDirectionMask.South) != 0)
            {
                enumerable.Add(CardinalDirectionMask.South);
            }

            return enumerable;
        }

        public static IEnumerable<CardinalDirection> GetDirections(this CardinalDirectionMask mask)
        {
            List<CardinalDirection> enumerable = new List<CardinalDirection>(4);

            if ((mask & CardinalDirectionMask.North) != 0)
            {
                enumerable.Add(CardinalDirection.North);
            }

            if ((mask & CardinalDirectionMask.East) != 0)
            {
                enumerable.Add(CardinalDirection.East);
            }

            if ((mask & CardinalDirectionMask.West) != 0)
            {
                enumerable.Add(CardinalDirection.West);
            }

            if ((mask & CardinalDirectionMask.South) != 0)
            {
                enumerable.Add(CardinalDirection.South);
            }

            return enumerable;
        }

        public static bool HasVertical(this CardinalDirectionMask mask) =>
            (mask & CardinalDirectionMask.North) != 0 || (mask & CardinalDirectionMask.South) != 0;

        public static bool HasHorizontal(this CardinalDirectionMask mask) =>
            (mask & CardinalDirectionMask.East) != 0 || (mask & CardinalDirectionMask.West) != 0;
    }

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
