using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class UnitManager: Manager
    {
        /// <summary>
        /// Holds all the enemy units currently in the level
        /// </summary>
        private readonly List<IUnit> enemyUnits = new List<IUnit>();
        
        /// <summary>
        /// Holds all the player units currently in the level
        /// </summary>
        private readonly List<IUnit> playerUnits = new List<IUnit>();

        /// <summary>
        /// Property that returns all the units currently in the level
        /// </summary>
        public IReadOnlyList<IUnit> AllUnits => GetAllUnits();
        
        /// <summary>
        /// Property that returns all enemy units in the level
        /// </summary>
        public IReadOnlyList<IUnit> EnemyUnits => enemyUnits.AsReadOnly();
        
        /// <summary>
        /// Property that returns all player units in the level
        /// </summary>
        public IReadOnlyList<IUnit> PlayerUnits => playerUnits.AsReadOnly();
        
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
        private List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(enemyUnits);
            allUnits.AddRange(playerUnits);
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
            foreach (PlayerUnit unit in playerUnits)
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
            foreach (EnemyUnit unit in enemyUnits)
            {
                if (unit.IsActing())
                    return unit;

            }
            return null; 
        }
        
        
        public void RemoveUnit(IUnit targetUnit) 
        {
            if (targetUnit is PlayerUnit)
            {
                if (playerUnits.Contains(targetUnit))
                    playerUnits.Remove(targetUnit);
                else
                    Debug.LogWarning("WARNING: Tried to remove " + targetUnit +
                                     " from EnemyManager but it isn't a part of the enemyUnits list");
            }
            else
            {
                if (enemyUnits.Contains(targetUnit))
                    enemyUnits.Remove(targetUnit);
                else
                    Debug.LogWarning("WARNING: Tried to remove " + targetUnit +
                                     " from EnemyManager but it isn't a part of the enemyUnits list");
            }
        }
        
        
        /// <summary>
        /// Clears all units of a certain IUnit type
        /// </summary>
        /// <typeparam name="T refers to IUnit type"></typeparam>
        public void ClearUnits<T>() where T : IUnit
        {
            T unit = default;
            
            if (unit is PlayerUnit)
                playerUnits.Clear();
            else
                enemyUnits.Clear();

        }
        
        /// <summary>
        /// Spawns in a unit prefab into the scene, takes in a prefab 
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The IUnit that was spawned</returns>
        public IUnit Spawn(GameObject unitPrefab, Vector2Int gridPosition)
        {
            return Spawn(UnitUtility.Spawn(unitPrefab, gridPosition));
        }
        
        /// <summary>
        /// Spawns in a unit prefab into the scene, takes in a unit name
        /// </summary>
        /// <param name="unitPrefab"></param>
        /// <param name="gridPosition"></param>
        /// <returns>The IUnit that was spawned</returns>
        public IUnit Spawn(string unitName, Vector2Int gridPosition)
        {
            return Spawn(UnitUtility.Spawn(unitName, gridPosition));
        }
        
        
        /// <summary>
        /// Spawns the unit in and adding them to their relevant unit list
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>The unit that was spawned</returns>
        private IUnit Spawn(IUnit unit)
        {
           if (unit is PlayerUnit)
               playerUnits.Add(unit);
           else
               enemyUnits.Add(unit);
           
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            return unit;
        } 
        
    }
    
        
    
    
}
