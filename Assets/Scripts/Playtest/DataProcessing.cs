using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Grid.GridObjects;
using Managers;
using TenetStatuses;
using Turn;
using Turn.Commands;
using Units;
using UnityEngine;

namespace Playtest
{
    public static class DataProcessing
    {
        private static readonly UnitManager unitManager = ManagerLocator.Get<UnitManager>();
        private static readonly TurnManager turnManager = ManagerLocator.Get<TurnManager>();

        private struct TemplateUnit
        {
            public int Amount { get; set; }

            public List<IUnit> Units { get; set; }

            public override string ToString()
            {
                string temp = "";
                
                foreach (IUnit unit in Units)
                    temp += unit.Name + " ";

                return temp + $"| {Amount}";
            }
        }
        
        public static string BattleOutcome(PlaytestData data)
        {
            int endHealthPool = 0;

            foreach (IUnit unit in unitManager.AllUnits)
            {
                endHealthPool += unit.HealthStat.Value;
            }

            float percentage;

            string battleOutcome;
            
            if (data.PlayerWin)
            {
                battleOutcome = " Victory";
                percentage = (float)endHealthPool / data.PlayerHealthPool * 100;
            }
            else
            {
                battleOutcome = " Defeat";
                percentage = (float)endHealthPool / data.EnemyHealthPool * 100;
            }

            if (percentage > 75)
                battleOutcome = "Total" + battleOutcome;
            else if (percentage >= 30 && percentage <= 74)
                battleOutcome = "Decisive" + battleOutcome;
            else if (percentage >= 10 && percentage <= 29)
                battleOutcome = "Close" + battleOutcome;
            else if (percentage < 9)
                battleOutcome = "Pyrrhic" + battleOutcome;

            return battleOutcome;
        }

        private static string UnitStatString(IUnit unit) =>
            unit.Name +
            " HP: " + unit.HealthStat.Value +
            " ATK: " + unit.AttackStat.Value +
            " DEF: " + unit.DefenceStat.Value +
            " MP: " + unit.MovementPoints.Value +
            " SPD: " + unit.SpeedStat.Value;

        private static string TenetStatusString(TenetStatus tenetStatus) =>
            tenetStatus.TenetType + " " + tenetStatus.StackCount;

        /// <summary>
        /// Converts a duration to a formatted duration string.
        /// </summary>
        /// <param name="duration">Duration in seconds.</param>
        /// <returns>Formatted duration string.</returns>
        private static string DurationString(float duration)
        {
            var timeSpan = TimeSpan.FromSeconds(duration);
            
            return $"{timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}";
        }

        private static string AbilityString(KeyValuePair<Ability, int> ability) =>
            $"{ability.Key.name} | {ability.Value}";

        private static string UnitTenetStatusString(IUnit unit)
        {
            string tenet1 = "";
            string tenet2 = "";

            var tenetStatusArray = unit.TenetStatuses.AsEnumerable().ToArray();

            if (tenetStatusArray.Length > 0)
                tenet1 = TenetStatusString(tenetStatusArray[0]);

            if (tenetStatusArray.Length > 1)
                tenet2 = TenetStatusString(tenetStatusArray[1]);

            return $"{tenet1} {tenet2}";
        }
        
        public static string FarthestMovedUnits(PlaytestData data)
        {

            var farthestMovedUnits = new TemplateUnit();
            
            // TODO: Repeated code. Will throw InvalidOperationException if Units is empty.
            farthestMovedUnits.Amount = data.Units.Max(u => u.DistanceMoved);
            farthestMovedUnits.Units = data.Units
                .Where(u => u.DistanceMoved == farthestMovedUnits.Amount)
                .Select(u => u.Unit)
                .ToList();
            
            return farthestMovedUnits + " cells";
        }

        public static string LeastTimesMovedUnits(PlaytestData data)
        {

            var leastTimesMovedUnits = new TemplateUnit();
            
            // TODO: Repeated code. Will throw InvalidOperationException if Units is empty.
            leastTimesMovedUnits.Amount = data.Units.Min(u => u.DistanceMoved);
            leastTimesMovedUnits.Units = data.Units
                .Where(u => u.DistanceMoved == leastTimesMovedUnits.Amount)
                .Select(u => u.Unit)
                .ToList();
            
            return leastTimesMovedUnits + " times";
        }

        public static string MostTimesMovedUnits(PlaytestData data)
        {
            var mostTimesMovedUnits = new TemplateUnit();
            
            // TODO: Repeated code. Will throw InvalidOperationException if Units is empty.
            mostTimesMovedUnits.Amount = data.Units.Max(u => u.TimesMoved);
            mostTimesMovedUnits.Units = data.Units
                .Where(u => u.TimesMoved == mostTimesMovedUnits.Amount)
                .Select(u => u.Unit)
                .ToList();
            
            return mostTimesMovedUnits + " times";
        }

        public static string MostTurnManipulatedUnits(PlaytestData data)
        {
            var mostTurnManipulatedUnits = new TemplateUnit();
            
            // TODO: Repeated code. Will throw InvalidOperationException if Units is empty.
            mostTurnManipulatedUnits.Amount = data.Units.Max(u => u.TimesTurnManipulated);
            mostTurnManipulatedUnits.Units = data.Units
                .Where(u => u.TimesTurnManipulated == mostTurnManipulatedUnits.Amount)
                .Select(u => u.Unit)
                .ToList();
            
            return mostTurnManipulatedUnits + " times";
        }

        public static string StrAverageTimesForTurns(PlaytestData data)
        {
            // TODO: Repeated code.
            float averageTimeForTurns = data.TimeForTurns.Values.AsQueryable().Average();
            string strAverageTimesForTurns = DurationString(averageTimeForTurns);
            
            return strAverageTimesForTurns;
        }

        public static string StrAverageTimeForRounds(PlaytestData data)
        {
            // TODO: Repeated code.
            float averageTimeForRounds = data.Rounds.Average(r => r.Time);
            string strAverageTimeForRounds = DurationString(averageTimeForRounds);

            return strAverageTimeForRounds;
        }
        
        // TODO: Test.
        public static string UpdateAbilityUsage(PlaytestData data)
        {
            var orderByDescending = data.Abilities.OrderByDescending(key => key
                .Value);

            var output = "Most used ability is ";
            
            foreach (var ability in orderByDescending)
                output += DataProcessing.AbilityString(ability) + Environment.NewLine;

            return output;
        }

        public static string TotalPlaytime(PlaytestData data) => DurationString(data.EncounterDuration);

        public static string EndStateUnits()
        {
            var output = "";
            
            foreach (IUnit unit in unitManager.AllUnits)
            {
                output += UnitStatString(unit) +
                          Environment.NewLine;
            }

            return output;
        }

        public static string RoundUnits()
        {
            var output = "";
            
            foreach (IUnit unit in unitManager.AllUnits)
            {
                output += UnitStatString(unit) + " " +
                          UnitTenetStatusString(unit) + " " +
                          Environment.NewLine;
            }

            return output;
        }

        public static string RoundEntry(PlaytestRoundData data)
        {
            var output = "";

            output += Environment.NewLine +
                      "CURRENT INSIGHT: " + turnManager.Insight.Value +
                      Environment.NewLine;


            output += Environment.NewLine + "Abilities used in this round were: ";

            foreach (Ability ability in data.AbilitiesUsed)
                output += Environment.NewLine + ability.name;

            return output;
        }
        
        public static string AbilityRoundAction(Ability ability, Vector2Int originCoord, Vector2 targetVector,
                                          IAbilityUser abilityUser)
        {
            var output = "";
            string targetNames = "";
            
            GridObject[] targets = ability.Shape.
                GetTargets(originCoord, targetVector).
                AsEnumerable().
                ToArray();

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] is IUnit unit)
                {
                    if (i == targets.Length - 1 || targets.Length == 1)
                        targetNames += unit.Name;
                    else
                        targetNames += unit.Name + " and ";
                }
            }

            output += $"{abilityUser.Name} casted {ability.name} at {targetNames}";

            // TODO: Add the effect of the ability to each affected unit here.

            output += Environment.NewLine;

            return output;
        }

        public static string TurnManipulatedRoundAction(IUnit unit, IUnit targetUnit)
        {
            return $"{unit} turn manipulated with {targetUnit}" + 
                               Environment.NewLine;
        }
        
        public static string MovementRoundAction(IUnit unit, Vector2Int startCoord, Vector2Int targetCoord)
        {
            return $"{unit.Name} moved from {startCoord} " +
                $"to {targetCoord}" + Environment.NewLine;
        }

        public static string MeditateRoundAction(MeditatedCommand cmd)
        {
            return $"{cmd.Unit} meditated" + Environment.NewLine;
        }

        public static string InitialUnits(PlaytestData data)
        {
            var initialUnits = "";

            foreach (var unit in data.Units)
            {
                initialUnits += UnitStatString(unit.Unit) +
                                " CORD: " + unit.Unit.Coordinate +
                                Environment.NewLine;
            }

            return initialUnits;
        }

        public static string EndRoundEntry(PlaytestRoundData roundData)
        {
            throw new NotImplementedException();
        }
    }
}
