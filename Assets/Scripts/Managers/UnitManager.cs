using System.Collections.Generic;
using Units;
using Units.Commands;
using UnityEngine;

namespace Managers
{
    public class UnitManager : Manager
    {
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
    }
}