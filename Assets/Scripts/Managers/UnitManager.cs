using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Units;
using UnityEngine;

namespace Managers
{
    
    public class UnitManager : Manager
    {

        private PlayerManager playerManager;

        private EnemyManager enemyManager;
        public override void ManagerStart()
        {
            playerManager = ManagerLocator.Get<PlayerManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
        }


        /// <summary>
        /// Get all the current player units in the game
        /// </summary>
        public List<IUnit> GetAllPlayerUnits()
        { 
            List<IUnit> playerUnits = new List<IUnit>();
            playerUnits.AddRange(playerManager.PlayerUnits);
           return playerUnits;
        }
        
        /// <summary>
        /// Get all the current enemy units in the game
        /// </summary>
        public List<IUnit> GetAllEnemyUnits()
        {
            List<IUnit> enemyUnits = new List<IUnit>();
            enemyUnits.AddRange(enemyManager.EnemyUnits);
            return enemyUnits;
        }
        
        /// <summary>
        /// Get all the units in the game
        /// </summary>
        public List<IUnit> GetAllUnits()
        {
            List<IUnit> allUnits = new List<IUnit>();
            allUnits.AddRange(GetAllPlayerUnits());
            allUnits.AddRange(GetAllEnemyUnits());
            return allUnits;
        }
        
        
        
        
    }

}