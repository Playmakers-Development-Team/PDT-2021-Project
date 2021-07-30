using System;
using System.Linq;
using TenetStatuses;

namespace Abilities
{
    [Flags]
    public enum TenetMask
    {
        None = 0,
        Pride = 1, 
        Humility = 2, 
        Passion = 4, 
        Apathy = 8, 
        Joy = 16, 
        Sorrow = 32
    }

    public static class TenetMaskExtensions
    {
        public static bool IsTenetInMask(this TenetMask tenetMask, TenetType tenetType) => tenetType switch 
        {
            TenetType.Pride => (tenetMask & TenetMask.Pride) != 0,
            TenetType.Humility => (tenetMask & TenetMask.Humility) != 0,
            TenetType.Passion => (tenetMask & TenetMask.Passion) != 0,
            TenetType.Apathy => (tenetMask & TenetMask.Apathy) != 0,
            TenetType.Joy => (tenetMask & TenetMask.Joy) != 0,
            TenetType.Sorrow => (tenetMask & TenetMask.Sorrow) != 0,
            _ => throw new ArgumentOutOfRangeException(nameof(tenetType), tenetType, null)
        };

        public static string ToDisplayName(this TenetMask tenetMask)
        {
            if (tenetMask == TenetMask.None)
                return "Nothing";
            
            string[] displayNames =
            {
                GetIndividualDisplayName(tenetMask, TenetType.Pride),
                GetIndividualDisplayName(tenetMask, TenetType.Humility),
                GetIndividualDisplayName(tenetMask, TenetType.Passion),
                GetIndividualDisplayName(tenetMask, TenetType.Apathy),
                GetIndividualDisplayName(tenetMask, TenetType.Joy),
                GetIndividualDisplayName(tenetMask, TenetType.Sorrow),
            };
            
            return string.Join(", ", displayNames.Where(s => !string.IsNullOrEmpty(s)));
        }

        private static string GetIndividualDisplayName(TenetMask tenetMask, TenetType tenetType) =>
            tenetMask.IsTenetInMask(tenetType) ? $"{tenetType}" : string.Empty;
    }
}
