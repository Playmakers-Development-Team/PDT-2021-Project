using System;
using System.Collections.Generic;

namespace Utility
{
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
            (mask & OrdinalDirectionMask.NorthEast) != 0 ||
            (mask & OrdinalDirectionMask.NorthWest) != 0 ||
            (mask & OrdinalDirectionMask.SouthEast) != 0 ||
            (mask & OrdinalDirectionMask.SouthWest) != 0;
    }
}
