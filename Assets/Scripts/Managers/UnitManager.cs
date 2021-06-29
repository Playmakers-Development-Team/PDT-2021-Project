using System.Collections.Generic;
using Units;
using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class UnitManager : Manager
    {
        public IUnit SelectedUnit { get; private set; }
        
        protected CommandManager commandManager;
        private EnemyManager enemyManager;
        private PlayerManager playerManager;

        /// <summary>
        /// All the units currently in the level.
        /// </summary>
        public IReadOnlyList<IUnit> AllUnits => GetAllUnits();

        /// <summary>
        /// Initialises the <c>UnitManager</c>.
        /// </summary>
        public override void ManagerStart()
        {
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
        }

        /// <summary>
        /// Returns all the units currently in the level.
        /// </summary>
        private List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(enemyManager.EnemyUnits);
            allUnits.AddRange(playerManager.PlayerUnits);
            return allUnits;
        }

        /// <summary>
        /// Removes a unit from the current timeline.
        /// </summary>
        /// <param name="targetUnit"></param>

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitPrefab, gridPosition);
            return unit;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(string unitName, Vector2Int gridPosition)
        {
            commandManager.ExecuteCommand(new SpawningUnitCommand());
            IUnit unit = UnitUtility.Spawn(unitName, gridPosition);
            return unit;
        }
        
        /// <summary>
        /// Sets a unit as selected.
        /// </summary>
        /// <param name="unit"></param>
        public void SelectUnit(IUnit unit)
        {
            if (unit is null)
            {
                Debug.LogWarning($"{nameof(UnitManager)}.{nameof(SelectUnit)} should not be " + 
                    $"passed a null value. Use {nameof(UnitManager)}.{nameof(DeselectUnit)} instead.");
                
                DeselectUnit();
                
                return;
            }

            if (SelectedUnit == unit)
                return;

            SelectedUnit = unit;
            commandManager.ExecuteCommand(new UnitSelectedCommand(SelectedUnit));
        }

        /// <summary>
        /// Deselects the currently selected unit.
        /// </summary>
        public void DeselectUnit()
        {
            if (SelectedUnit is null)
                return;
            
            Debug.Log(SelectedUnit + " deselected.");
            commandManager.ExecuteCommand(new UnitDeselectedCommand(SelectedUnit));
            SelectedUnit = null;
        }
    }
}