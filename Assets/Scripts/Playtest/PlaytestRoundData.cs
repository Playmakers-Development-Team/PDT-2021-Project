using System.Collections.Generic;
using Abilities;

namespace Playtest
{
    public class PlaytestRoundData
    {
        public int CurrentInsight { get; set; }
        public float Time { get; set; }
        public List<Ability> AbilitiesUsed { get; } = new List<Ability>();
        public string RoundUnits { get; set; }
        public List<PlaytestRoundActionData> RoundActions { get; } =
            new List<PlaytestRoundActionData>();
    }
}
