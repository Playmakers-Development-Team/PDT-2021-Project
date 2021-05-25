using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    public enum QuadDirection
    {
        Up, Right, Down, Left
    }

    public static class QuadDirectionExtensions
    {
        public static QuadDirection Opposite(this QuadDirection direction)
        {
            return (QuadDirection) (((int) direction + 2) % 4);
        }

        public static QuadDirection RotateClockwise(this QuadDirection direction)
        {
            return (QuadDirection) (((int) direction + 1) % 4);
        }
        
        public static QuadDirection RotateAntiClockwise(this QuadDirection direction)
        {
            int directionIndex = (int) direction - 1;
            return (QuadDirection) (directionIndex < 0 ? 3 : directionIndex % 4);
        }
        
        public static Vector2Int ToVector2Int(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.Right:
                    return Vector2Int.right;
                case QuadDirection.Down:
                    return Vector2Int.down;
                case QuadDirection.Left:
                    return Vector2Int.left;
                default:
                    return Vector2Int.up;
            }
        }

        public static Vector2 ToVector2(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.Right:
                    return Vector2.right;
                case QuadDirection.Down:
                    return Vector2.down;
                case QuadDirection.Left:
                    return Vector2.left;
                default:
                    return Vector2.up;
            }
        }

        public static QuadDirectionMask ToMask(this QuadDirection direction)
        {
            switch (direction)
            {
                case QuadDirection.Right:
                    return QuadDirectionMask.Right;
                case QuadDirection.Down:
                    return QuadDirectionMask.Down;
                case QuadDirection.Left:
                    return QuadDirectionMask.Left;
                default:
                    return QuadDirectionMask.Up;
            }
        }
    }

    [Flags]
    public enum QuadDirectionMask
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
    }

    public static class QuadDirectionMaskExtensions
    {
        public static bool Contains(this QuadDirectionMask mask, QuadDirection direction)
        {
            return (mask & direction.ToMask()) != 0;
        }
        
        public static IEnumerable<QuadDirectionMask> Split(this QuadDirectionMask mask)
        {
            List<QuadDirectionMask> enumerable = new List<QuadDirectionMask>(4);
            
            if ((mask & QuadDirectionMask.Up) != 0)
            {
                enumerable.Add(QuadDirectionMask.Up);
            }
            
            if ((mask & QuadDirectionMask.Right) != 0)
            {
                enumerable.Add(QuadDirectionMask.Right);
            }
            
            if ((mask & QuadDirectionMask.Left) != 0)
            {
                enumerable.Add(QuadDirectionMask.Left);
            }
            
            if ((mask & QuadDirectionMask.Down) != 0)
            {
                enumerable.Add(QuadDirectionMask.Down);
            }

            return enumerable;
        }
        
        public static IEnumerable<QuadDirection> GetDirections(this QuadDirectionMask mask)
        {
            List<QuadDirection> enumerable = new List<QuadDirection>(4);
            
            if ((mask & QuadDirectionMask.Up) != 0)
            {
                enumerable.Add(QuadDirection.Up);
            }
            
            if ((mask & QuadDirectionMask.Right) != 0)
            {
                enumerable.Add(QuadDirection.Right);
            }
            
            if ((mask & QuadDirectionMask.Left) != 0)
            {
                enumerable.Add(QuadDirection.Left);
            }
            
            if ((mask & QuadDirectionMask.Down) != 0)
            {
                enumerable.Add(QuadDirection.Down);
            }

            return enumerable;
        }
    }

    public static class QuadDirectionUtil
    {
        public static QuadDirection From(Vector2 fromPos, Vector2 toPos)
        {
            Vector2 vector = (toPos - fromPos).normalized;
            float angle = Vector2.SignedAngle(Vector2.up, vector);

            if (angle >= 45 && angle < 135)
            {
                return QuadDirection.Left;
            }
            
            if (angle < -45 && angle >= -135)
            {
                return QuadDirection.Right;
            } 
            
            if (angle >= -45 && angle < 45)
            {
                return QuadDirection.Up;
            }

            return QuadDirection.Down;
        }

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