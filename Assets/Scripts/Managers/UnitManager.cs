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
        /// Property that returns all the units currently in the level
        /// </summary>
        public IReadOnlyList<IUnit> AllUnits => GetAllUnits();

        /// <summary>
        /// Get the current "ACTING" player unit
        /// </summary>
        public IUnit GetCurrentActingPlayerUnit => GetActivePlayerUnit();

        /// <summary>
        /// Get the current "ACTING" enemy unit
        /// </summary>
        public IUnit GetCurrentActiveEnemyUnit => GetActiveEnemyUnit();

        /// <summary>
        /// Get the current "ACTING" unit
        /// </summary>
        public IUnit GetCurrentActiveUnit => GetActiveUnit();

        /// <summary>
        /// Get all the units in the game.
        /// </summary>
        ///
        public override void ManagerStart()
        {
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
        }

        private List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(enemyManager.EnemyUnits);
            allUnits.AddRange(playerManager.PlayerUnits);
            return allUnits;
        }

        /// <summary>
        /// Get all the current active unit
        /// </summary>
        private IUnit GetActiveUnit()
        {
            foreach (IUnit unit in AllUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            return null;
        }

        /// <summary>
        /// Get the current player active unit
        /// </summary>
        private PlayerUnit GetActivePlayerUnit()
        {
            foreach (PlayerUnit unit in playerManager.PlayerUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            return null;
        }

        /// <summary>
        /// Get the current active enemy unit
        /// </summary>
        private EnemyUnit GetActiveEnemyUnit()
        {
            foreach (EnemyUnit unit in enemyManager.EnemyUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            return null;
        }

        /// <summary>
        /// A function that removes a certain unit from the current timeline
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual void RemoveUnit(IUnit targetUnit) =>
            ManagerLocator.Get<TurnManager>().RemoveUnitFromQueue(targetUnit);

        /// <summary>
        /// A function that Spawns a target unit
        /// </summary>
        /// <param name="targetUnit"></param>
        public virtual IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(unitPrefab, gridPosition);
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            return unit;
        }
    }
}
