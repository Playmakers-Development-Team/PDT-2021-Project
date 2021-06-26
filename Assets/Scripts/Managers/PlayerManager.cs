using System.Collections.Generic;
using Units;
using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : UnitManager
    {
        private readonly List<IUnit> playerUnits = new List<IUnit>();

        /// <summary>
        /// All the player units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> PlayerUnits => playerUnits.AsReadOnly();
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay { get; } = 1000;
        public int Count => playerUnits.Count;

        /// <summary>
        /// Removes all the player units in the <c>playerUnits</c> list.
        /// </summary>
        public void ClearPlayerUnits() => playerUnits.Clear();

        /// <summary>
        /// Removes a target <c>IUnit</c> from <c>playerUnits</c>.
        /// </summary>
        /// <param name="targetUnit"></param>
        public void RemoveUnit(IUnit targetUnit) => playerUnits.Remove(targetUnit);
        public void AddUnit(IUnit targetUnit) => playerUnits.Add(targetUnit);

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
        public IUnit Spawn(PlayerUnit unit)
        {
            playerUnits.Add(unit);
            
            commandManager.ExecuteCommand(new SpawnedUnitCommand(unit));
            
            SelectUnit(unit);
            
            return unit;
        }
    }
}
