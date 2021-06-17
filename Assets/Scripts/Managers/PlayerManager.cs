using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : UnitManager
    {
        /// <summary>
        /// Holds the current SelectedUnit in the scene
        /// </summary>
        public IUnit SelectedUnit { get; private set; }

        /// <summary>
        /// Holds all the player units currently in the level
        /// </summary>
        private readonly List<IUnit> playerUnits = new List<IUnit>();

        /// <summary>
        /// Property that returns all player units in the level
        /// </summary>
        public IReadOnlyList<IUnit> PlayerUnits => playerUnits.AsReadOnly();

        /// <summary>
        /// A function that removes all the player units in the playerUnits list
        /// </summary>
        public void ClearPlayerUnits() => playerUnits.Clear();

        /// <summary>
        /// A function that removes a target IUnit
        /// </summary>
        /// <param name="targetUnit"></param>
        public override void RemoveUnit(IUnit targetUnit)
        {
            playerUnits.Remove(targetUnit);
            base.RemoveUnit(targetUnit);
        }

        /// <summary>
        /// Spawns a player unit and adds them to the playerUnits list
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The new IUnit</returns>
        public override IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit newUnit = base.Spawn(unitPrefab, gridPosition);
            playerUnits.Add(newUnit);
            return newUnit;
        }

        /// <summary>
        /// Selects a unit to determine if it has been clicked or not (for UI purposes)
        /// </summary>
        /// <param name="unit"></param>
        public void SelectUnit(PlayerUnit unit)
        {
            if (unit is null)
            {
                Debug.LogWarning(
                    "PlayerManager.SelectUnit should not be passed a null value. Use PlayerManager.DeselectUnit instead.");
                DeselectUnit();
                return;
            }

            if ((PlayerUnit) SelectedUnit != unit)
            {
                SelectedUnit = unit;

                ManagerLocator.Get<CommandManager>().
                    ExecuteCommand(new Commands.UnitSelectedCommand(SelectedUnit));
            }
        }

        /// <summary>
        /// Deselects a unit, making any UI prompts disappear
        /// </summary>
        public void DeselectUnit()
        {
            SelectedUnit = null;
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.UnitDeselectedCommand(SelectedUnit));
        }
    }
}
