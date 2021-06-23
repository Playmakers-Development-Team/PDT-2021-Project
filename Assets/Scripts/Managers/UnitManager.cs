using System;
using System.Collections.Generic;
using Units;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine.Assertions.Must;

namespace Managers
{
    public class UnitManager : Manager
    {
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private UnitManager unitManager;
        
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

        public override void ManagerStart()
        {
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            unitManager = ManagerLocator.Get<UnitManager>();
        }

        /// <summary>
        /// Get all the current player units in the game.
        /// </summary>
        public List<IUnit> GetAllPlayerUnits()
        {
            List<IUnit> playerUnits = new List<IUnit>();
            playerUnits.AddRange(playerManager.PlayerUnits);
            return playerUnits;
        }

        /// <summary>
        /// Get all the current enemy units in the game.
        /// </summary>
        public List<IUnit> GetAllEnemyUnits()
        {
            List<IUnit> enemyUnits = new List<IUnit>();
            enemyUnits.AddRange(enemyManager.EnemyUnits);
            return enemyUnits;
        }

        /// <summary>
        /// Get all the units in the game.
        /// </summary>
        public List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(GetAllPlayerUnits());
            allUnits.AddRange(GetAllEnemyUnits());
            return allUnits;
        }

    
        
        /// <summary>
        /// Get all the current active unit
        /// </summary>
        private IUnit GetActiveUnit()
        {
            foreach (IUnit unit in unitManager.GetAllUnits())
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
            foreach (PlayerUnit unit in unitManager.GetAllPlayerUnits())
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
            foreach (EnemyUnit unit in unitManager.GetAllEnemyUnits())
            {
                if (unit.IsActing())
                    return unit;

            }
            return null; 
        }
        
        
        
        
    }
}
