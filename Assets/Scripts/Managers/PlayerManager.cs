using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : UnitManager
    {
        
        public IUnit SelectedUnit { get; private set; }
        private readonly List<IUnit> playerUnits = new List<IUnit>();

        /// <summary>
        /// All the player units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> PlayerUnits => playerUnits.AsReadOnly();
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay {get;} = 1000; 
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
        

        /// <summary>
        /// Spawns a player unit and adds it to the <c>playerUnits</c> list.
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new <c>IUnit</c>.</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
           // playerUnits.Add(newUnit);
            return newUnit;
        }

        /// <summary>
        /// Sets a unit as selected.
        /// </summary>
        /// <param name="unit"></param>
        public void SelectUnit(PlayerUnit unit)
        {
            if (WaitForDeath) return;
            if (unit is null)
            {
                Debug.LogWarning(
                    "PlayerManager.SelectUnit should not be passed a null value. Use PlayerManager.DeselectUnit instead.");
                DeselectUnit();
                return;
            }

            if (SelectedUnit != unit)
            {
                SelectedUnit = unit;

                ManagerLocator.Get<CommandManager>().
                    ExecuteCommand(new Commands.UnitSelectedCommand(SelectedUnit));
            }
        }

        /// <summary>
        /// Deselects the currently selected unit.
        /// </summary>
        public void DeselectUnit()
        {
            SelectedUnit = null;
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.UnitDeselectedCommand(SelectedUnit));
        }
        
        /// <summary>
        /// Adds an already existing unit to the <c>playerUnits</c> list. Currently used by units
        /// that have been added to the scene in the editor.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        public IUnit Spawn(PlayerUnit unit)
        {
            playerUnits.Add(unit);
            
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            
            SelectUnit(unit);
            
            return unit;
        }
    }
}
