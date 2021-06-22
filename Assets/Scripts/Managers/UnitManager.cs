using System;
using System.Collections.Generic;
using Commands;
using JetBrains.Annotations;
using Units;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

namespace Managers
{
    public class UnitManager : Manager
    {
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
        public virtual void RemoveUnit(IUnit targetUnit) =>
            ManagerLocator.Get<TurnManager>().RemoveUnitFromQueue(targetUnit);

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(unitPrefab, gridPosition);
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            return unit;
        }

        /// <summary>
        /// Spawns a unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(string unitName, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(unitName, gridPosition);
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            return unit;
        }
    }
}