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
}
