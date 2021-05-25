using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public enum Direction
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

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction direction) =>
            (Direction) (((int) direction + 4) % 8);

        public static Direction RotateClockwise(this Direction direction) =>
            (Direction) (((int) direction + 1) % 8);

        public static Direction RotateAntiClockwise(this Direction direction)
        {
            int directionIndex = (int) direction - 1;
            return (Direction) (directionIndex < 0 ? 7 : directionIndex % 8);
        }

        public static Vector2Int ToVector2Int(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Vector2Int.up;
                case Direction.NorthEast:
                    return Vector2Int.up + Vector2Int.right;
                case Direction.East:
                    return Vector2Int.right;
                case Direction.SouthEast:
                    return Vector2Int.down + Vector2Int.right;
                case Direction.South:
                    return Vector2Int.down;
                case Direction.SouthWest:
                    return Vector2Int.down + Vector2Int.left;
                case Direction.West:
                    return Vector2Int.left;
                case Direction.NorthWest:
                    return Vector2Int.up + Vector2Int.left;
            }

            throw new Exception($"{nameof(Direction)} {direction} is not handled");
        }

        public static Vector2 ToVector2(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Vector2.up;
                case Direction.NorthEast:
                    return Vector2.up + Vector2.right;
                case Direction.East:
                    return Vector2.right;
                case Direction.SouthEast:
                    return Vector2.down + Vector2.right;
                case Direction.South:
                    return Vector2.down;
                case Direction.SouthWest:
                    return Vector2.down + Vector2.left;
                case Direction.West:
                    return Vector2.left;
                case Direction.NorthWest:
                    return Vector2.up + Vector2.left;
            }

            throw new Exception($"{nameof(Direction)} {direction} is not handled");
        }

        public static DirectionMask ToMask(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return DirectionMask.North;
                case Direction.NorthEast:
                    return DirectionMask.NorthEast;
                case Direction.East:
                    return DirectionMask.East;
                case Direction.SouthEast:
                    return DirectionMask.SouthEast;
                case Direction.South:
                    return DirectionMask.South;
                case Direction.SouthWest:
                    return DirectionMask.SouthWest;
                case Direction.West:
                    return DirectionMask.West;
                case Direction.NorthWest:
                    return DirectionMask.NorthWest;
            }

            throw new Exception($"{nameof(Direction)} {direction} is not handled");
        }

        public static QuadDirection ToQuadDirection(this Direction direction)
        {
            Direction newDirection = direction;

            if (direction.IsDiagonal())
            {
                newDirection = newDirection.RotateClockwise();
            }

            switch (newDirection)
            {
                case Direction.East:
                    return QuadDirection.East;
                case Direction.South:
                    return QuadDirection.South;
                case Direction.West:
                    return QuadDirection.West;
                default:
                    return QuadDirection.North;
            }
        }

        public static bool IsHorizontal(this Direction direction) =>
            direction == Direction.East || direction == Direction.West;

        public static bool IsVertical(this Direction direction) =>
            direction == Direction.North || direction == Direction.South;

        public static bool IsDiagonal(this Direction direction) =>
            direction == Direction.NorthEast || direction == Direction.NorthWest
                                             || direction == Direction.SouthWest
                                             || direction == Direction.SouthEast;
    }

    [Flags]
    public enum DirectionMask
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

    public static class DirectionMaskExtensions
    {
        public static bool Contains(this DirectionMask mask, Direction direction) =>
            (mask & direction.ToMask()) != 0;

        public static IEnumerable<DirectionMask> Split(this DirectionMask mask)
        {
            List<DirectionMask> enumerable = new List<DirectionMask>(4);

            if ((mask & DirectionMask.North) != 0)
            {
                enumerable.Add(DirectionMask.North);
            }

            if ((mask & DirectionMask.NorthEast) != 0)
            {
                enumerable.Add(DirectionMask.NorthEast);
            }

            if ((mask & DirectionMask.East) != 0)
            {
                enumerable.Add(DirectionMask.East);
            }

            if ((mask & DirectionMask.SouthEast) != 0)
            {
                enumerable.Add(DirectionMask.SouthEast);
            }

            if ((mask & DirectionMask.South) != 0)
            {
                enumerable.Add(DirectionMask.South);
            }

            if ((mask & DirectionMask.SouthWest) != 0)
            {
                enumerable.Add(DirectionMask.SouthWest);
            }

            if ((mask & DirectionMask.West) != 0)
            {
                enumerable.Add(DirectionMask.West);
            }

            if ((mask & DirectionMask.NorthWest) != 0)
            {
                enumerable.Add(DirectionMask.NorthWest);
            }

            return enumerable;
        }

        public static IEnumerable<Direction> GetDirections(this DirectionMask mask)
        {
            List<Direction> enumerable = new List<Direction>(4);

            if ((mask & DirectionMask.North) != 0)
            {
                enumerable.Add(Direction.North);
            }

            if ((mask & DirectionMask.NorthEast) != 0)
            {
                enumerable.Add(Direction.NorthEast);
            }

            if ((mask & DirectionMask.East) != 0)
            {
                enumerable.Add(Direction.East);
            }

            if ((mask & DirectionMask.SouthEast) != 0)
            {
                enumerable.Add(Direction.SouthEast);
            }

            if ((mask & DirectionMask.South) != 0)
            {
                enumerable.Add(Direction.South);
            }

            if ((mask & DirectionMask.SouthWest) != 0)
            {
                enumerable.Add(Direction.SouthWest);
            }

            if ((mask & DirectionMask.West) != 0)
            {
                enumerable.Add(Direction.West);
            }

            if ((mask & DirectionMask.NorthWest) != 0)
            {
                enumerable.Add(Direction.NorthWest);
            }

            return enumerable;
        }

        public static bool HasVertical(this DirectionMask mask) =>
            (mask & DirectionMask.North) != 0 || (mask & DirectionMask.South) != 0;

        public static bool HasHorizontal(this DirectionMask mask) =>
            (mask & DirectionMask.East) != 0 || (mask & DirectionMask.West) != 0;

        public static bool HasDiagonal(this DirectionMask mask) =>
            (mask & DirectionMask.NorthEast) != 0 || (mask & DirectionMask.NorthWest) != 0
                                                  || (mask & DirectionMask.SouthEast) != 0
                                                  || (mask & DirectionMask.SouthWest) != 0;
    }

    public static class DirectionUtil
    {
        public static Direction From(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 vector = (toPos - fromPos).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, vector);

            if (angle >= 22.5f && angle < 67.5f)
            {
                return Direction.NorthWest;
            }

            if (angle >= 67.5f && angle < 112.5f)
            {
                return Direction.West;
            }

            if (angle >= 112.5f && angle < 157.5f)
            {
                return Direction.SouthWest;
            }

            if (angle < -22.5f && angle >= -67.5f)
            {
                return Direction.NorthEast;
            }

            if (angle < -67.5f && angle >= -112.5f)
            {
                return Direction.East;
            }

            if (angle < -112.5f && angle >= -157.5f)
            {
                return Direction.SouthEast;
            }

            if (angle >= -22.5f && angle < 22.5f)
            {
                return Direction.North;
            }

            return Direction.South;
        }

        public static DirectionMask GetMaskFrom(params Direction[] directions) => 
            GetMaskFrom(directions.AsEnumerable());

        public static DirectionMask GetMaskFrom(IEnumerable<Direction> directions)
        {
            DirectionMask mask = DirectionMask.None;

            foreach (Direction direction in directions)
            {
                mask |= direction.ToMask();
            }

            return mask;
        }
    }
}
