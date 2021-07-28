using System;
using System.Collections.Generic;
using Abilities;
using Units;
using UnityEngine;

namespace Playtest
{
    [CreateAssetMenu(menuName = "PlaytestData")]
    public class PlaytestData : ScriptableObject
    {
        public string ActiveScene;
        public int TimesMoved { get; set; }
        public Dictionary<Ability, int> Abilities { get; } = new Dictionary<Ability, int>();
        public Dictionary<IUnit, int> UnitsTurnManipulated { get; } = new Dictionary<IUnit, int>();
        public Dictionary<IUnit, int> UnitsMoved { get; } = new Dictionary<IUnit, int>();
        public Dictionary<IUnit, int> UnitsMovedLeast { get; } = new Dictionary<IUnit, int>();
        public Dictionary<IUnit, int> UnitsMovedDistance { get; } = new Dictionary<IUnit, int>();
        public Dictionary<int, float> TimeForRounds { get; } = new Dictionary<int, float>();
        public Dictionary<int, float> TimeForTurns { get; } = new Dictionary<int, float>();
        public List<Ability> AbilitiesUsedInARound { get; } = new List<Ability>();
        public string InitialUnits { get; set; }
        public string EndStateUnits { get; set; }
        public string AbilityUsage { get; set; }
        public int PlayerHealthPool { get; set; }
        public int EnemyHealthPool { get; set; }
        public string TurnManipulationData { get; set; }
        public int AmountOfTurnsManipulated { get; set; }
        public int MeditateAmount { get; set; }
        public int RoundCount { get; set; }
        public int TurnCount { get; set; }
        public string RoundEntry { get; set; }
        public string BattleOutcome { get; set; }
        public float OverallTime { get; set; }
        public float RoundTimer { get; set; }
        public float TurnTimer { get; set; }
        public List<Tuple<string, string>> Entries { get; } = new List<Tuple<string, string>>();
        public string InitialUnitOrder { get; set; }
    }
}
