using System;
using System.Collections.Generic;

namespace Utility
{
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
}
