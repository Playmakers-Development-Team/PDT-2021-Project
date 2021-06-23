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
        /// The unit whose turn it currently is. Is null if no unit is acting.
        /// </summary>
        public IUnit ActingUnit => GetActingUnit();
        
        /// <summary>
        /// The <c>PlayerUnit</c> whose turn it currently is. Is null if none
        /// <c>PlayerUnit</c> is acting.
        /// </summary>
        public PlayerUnit ActingPlayerUnit => GetActingPlayerUnit();

        /// <summary>
        /// The <c>EnemyUnit</c> whose turn it currently is. Is null if none
        /// <c>EnemyUnit</c> is acting.
        /// </summary>
        public EnemyUnit ActingEnemyUnit => GetActingEnemyUnit();

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
        /// Returns the unit whose turn it currently is. Returns null if no
        /// unit is acting. 
        /// </summary>
        private IUnit GetActingUnit()
        {
            foreach (IUnit unit in AllUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            Debug.LogWarning("No unit is currently acting.");
            return null;
        }

        /// <summary>
        /// Returns the <c>PlayerUnit</c> whose turn it currently is. Returns null if no
        /// <c>PlayerUnit</c> is acting. 
        /// </summary>
        private PlayerUnit GetActingPlayerUnit()
        {
            foreach (PlayerUnit unit in playerManager.PlayerUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            return null;
        }

        /// <summary>
        /// Returns the <c>EnemyUnit</c> whose turn it currently is. Returns null if no
        /// <c>EnemyUnit</c> is acting. 
        /// </summary>
        private EnemyUnit GetActingEnemyUnit()
        {
            foreach (EnemyUnit unit in enemyManager.EnemyUnits)
            {
                if (unit.IsActing())
                    return unit;
            }

            return null;
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