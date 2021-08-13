using System.Collections.Generic;
using System.Linq;
using Abilities;
using UnityEngine;

namespace Playtest
{
    // TODO: Stores both raw and processed data.
    [CreateAssetMenu(menuName = "PlaytestData")]
    public class PlaytestData : ScriptableObject
    {
        public string ActiveScene { get; set; }
        public Dictionary<Ability, int> Abilities { get; } = new Dictionary<Ability, int>();
        public List<PlaytestUnitData> Units { get; } = new List<PlaytestUnitData>();
        public Dictionary<int, float> TimeForTurns { get; } = new Dictionary<int, float>();
        public string EndStateUnits { get; set; } = "";
        public int PlayerHealthPool { get; set; }
        public int EnemyHealthPool { get; set; }
        public int AmountOfTurnsManipulated { get; set; }
        public int MeditateAmount { get; set; }
        public bool PlayerWin { get; set; }
        public string InitialUnits { get; set; }
        public float EncounterDuration { get; set; }
        public List<PlaytestRoundData> Rounds { get; } = new List<PlaytestRoundData>();
        public string InitialUnitOrder { get; set; }
        public int TurnCount { get; set; }
        public PlaytestRoundData CurrentRound => Rounds.Last();
    }
}
