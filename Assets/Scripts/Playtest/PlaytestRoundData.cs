using System.Collections.Generic;
using Abilities;

namespace Playtest
{
    public class PlaytestRoundData
    {
        public PlaytestRoundData(int currentInsight, string roundUnits)
        {
            CurrentInsight = currentInsight;
            RoundUnits = roundUnits;
        }

        public int CurrentInsight { get; }
        public float Time { get; set; }
        public List<Ability> AbilitiesUsed { get; } = new List<Ability>();
        public string RoundUnits { get; }

        public List<PlaytestRoundActionData> RoundActions { get; } =
            new List<PlaytestRoundActionData>();
    }
}
