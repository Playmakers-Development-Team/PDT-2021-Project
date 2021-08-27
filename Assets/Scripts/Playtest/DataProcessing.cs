using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Abilities.Commands;
using Grid.GridObjects;
using Managers;
using TenetStatuses;
using Turn.Commands;
using Units;
using UnityEngine;

namespace Playtest
{
    public static class DataProcessing
    {
        private static readonly UnitManager unitManager = ManagerLocator.Get<UnitManager>();

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
            " HP: " + unit.HealthStat +
            " ATK: " + unit.AttackStat +
            " DEF: " + unit.DefenceStat +
            " MP: " + unit.MovementPoints +
            " SPD: " + unit.SpeedStat;

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
            
            return $"{timeSpan.TotalHours:00}:{timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}";
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
        
        public static string FarthestMovedUnits(PlaytestData data) =>
            AllUnitsWithPropertyValue(
                data,
                int.Parse(MaxUnitMovementDistance(data)),
                u => u.DistanceMoved,
                "cells"
            );
        
        public static string MaxUnitMovementDistance(PlaytestData data) =>
            FindHeuristicProperty(
                data,
                Enumerable.Max,
                u => u.DistanceMoved
            ).ToString();

        public static string LeastTimesMovedUnits(PlaytestData data) =>
            AllUnitsWithPropertyValue(
                data,
                int.Parse(MinUnitMovements(data)),
                u => u.TimesMoved,
                "times"
            );
        
        public static string MinUnitMovements(PlaytestData data) =>
            FindHeuristicProperty(
                data,
                Enumerable.Min,
                u => u.TimesMoved
            ).ToString();

        public static string MostTimesMovedUnits(PlaytestData data) =>
            AllUnitsWithPropertyValue(
                data,
                int.Parse(MaxUnitMovements(data)),
                u => u.TimesMoved,
                "times"
            );
        
        public static string MaxUnitMovements(PlaytestData data) =>
            FindHeuristicProperty(
                data,
                Enumerable.Max,
                u => u.TimesMoved
            ).ToString();

        public static string MostTurnManipulatedUnits(PlaytestData data) =>
            AllUnitsWithPropertyValue(
                data,
                int.Parse(MaxUnitTurnManipulations(data)),
                u => u.TimesTurnManipulated,
                "times"
            );

        public static string MaxUnitTurnManipulations(PlaytestData data) =>
            FindHeuristicProperty(
                data,
                Enumerable.Max,
                u => u.TimesTurnManipulated
            ).ToString();

        private static string AllUnitsWithPropertyValue(
            PlaytestData data,
            int propertyValue,
            Func<PlaytestUnitData, int> propertyAccessor,
            string amountSuffix
        )
        {
            if (data.Units.Count == 0)
                return "";
            
            var units = data.Units
                .Where(u => propertyAccessor(u) == propertyValue)
                .Select(u => u.Unit)
                .ToList();
            
            return $"{UnitListString(units)} | {propertyValue} {amountSuffix}";
        }
        
        private static int FindHeuristicProperty(
            PlaytestData data,
            Func<IEnumerable<PlaytestUnitData>, Func<PlaytestUnitData, int>, int> heuristic,
            Func<PlaytestUnitData, int> propertyAccessor
        ) =>
            heuristic(data.Units, propertyAccessor);

        private static string UnitListString(List<IUnit> units) => string.Join(", ", units.Select(u => u.Name));

        public static string StrAverageTimesForTurns(PlaytestData data)
        {
            if (data.TimeForTurns.Count == 0)
                return "N/A";
            
            float averageTimeForTurns = data.TimeForTurns.Values.AsQueryable().Average();
            return DurationString(averageTimeForTurns);
        }

        public static string StrAverageTimeForRounds(PlaytestData data)
        {
            if (data.Rounds.Count == 0)
                return "N/A";
            
            float averageTimeForRounds = data.Rounds.Average(r => r.Time);
            return DurationString(averageTimeForRounds);
        }
        
        // TODO: Can this be refactored to use AllUnitsWithHeuristicProperty?
        public static string UpdateAbilityUsage(PlaytestData data)
        {
            var orderByDescending = data.Abilities.OrderByDescending(key => key
                .Value);

            var output = "Most used ability is ";
            
            foreach (var ability in orderByDescending)
                output += AbilityString(ability) + Environment.NewLine;

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

        public static string AbilityRoundAction(AbilityCommand cmd)
        {
            var output = "";
            string targetNames = "";
            
            GridObject[] targets = cmd.Ability.Shape.
                GetTargets(cmd.OriginCoordinate, cmd.TargetVector).
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

            output += $"{cmd.AbilityUser.Name} casted {cmd.Ability.name} at {targetNames}";

            // TODO: Add the effect of the ability to each affected unit here.

            output += Environment.NewLine;

            return output;
        }

        public static string TurnManipulatedRoundAction(IUnit unit, IUnit targetUnit) =>
            $"{unit} turn manipulated with {targetUnit}" + 
            Environment.NewLine;

        public static string MovementRoundAction(IUnit unit, Vector2Int startCoord, Vector2Int targetCoord) =>
            $"{unit.Name} moved from {startCoord} " +
            $"to {targetCoord}" + Environment.NewLine;

        public static string MeditateRoundAction(MeditatedCommand cmd) =>
            $"{cmd.Unit} meditated" + Environment.NewLine;

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
            var output = "";
            
            // Units
            output += roundData.RoundUnits + Environment.NewLine;

            // Insight
            output += Environment.NewLine +
                      "CURRENT INSIGHT: " + roundData.CurrentInsight +
                      Environment.NewLine;
            
            // Round Actions
            foreach (var roundAction in roundData.RoundActions)
                output += roundAction.RoundAction + Environment.NewLine;
            
            // Abilities
            output += Environment.NewLine + "Abilities used in this round were: ";

            foreach (Ability ability in roundData.AbilitiesUsed)
                output += Environment.NewLine + ability.name;
            
            return output;
        }
    }
}
