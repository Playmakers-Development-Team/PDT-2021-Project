using System;
using System.Collections.Generic;
using Units.Commands;
using Units.Stats;
using UnityEngine;

namespace Units.Players
{
    public class PlayerManager : UnitManager
    {
        /// <summary>
        /// All the player units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> PlayerUnits => units.AsReadOnly();
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay { get; } = 1000;
        public int Count => units.Count;

        [Obsolete("Use Insight of type 'Stat' instead from the TurnManager")]
        public ValueStat Insight { get; private set; }
        
        public override void ManagerStart()
        {
            base.ManagerStart();
            
            // TODO: The base value of insight might want to be exposed somewhere
            // TODO: ValueStat constructor should take BaseValue as an argument
            Insight = new ValueStat {BaseValue = 0};
            Insight.Reset();
        }

        /// <summary>
        /// Removes all the player units in the <c>playerUnits</c> list.
        /// </summary>
        public void ClearPlayerUnits() => units.Clear();

        /// <summary>
        /// Removes a target <c>IUnit</c> from <c>playerUnits</c>.
        /// </summary>
        /// <param name="targetUnit"></param>
        public void RemoveUnit(IUnit targetUnit) => units.Remove(targetUnit);
        
        /// <summary>
        /// Adds a unit to the <c>playerUnits</c> list.
        /// </summary>
        public void AddUnit(IUnit targetUnit) => units.Add(targetUnit);
        
        // TODO: Duplicate code, see EnemyManager.ClearUnits. Should use generics for unit managers.
        public void ClearUnits() => units.Clear();

        /// <summary>
        /// Spawns a player unit and adds it to the <c>playerUnits</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c>.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
            return newUnit;
        }

        /// <summary>
        /// Adds an already existing unit to the <c>playerUnits</c> list. Currently used by units
        /// that have been added to the scene in the editor.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        public override IUnit Spawn(IUnit unit)
        {
            base.Spawn(unit);
            
            SelectUnit(unit);
            
            return unit;
        }
    }
}
