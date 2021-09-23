using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Abilities;
using UnityEngine;

namespace Units.Players
{
    public class PlayerManager : UnitManager<PlayerUnitData>
    {
        public AbilityPool AbilityPickupPool { get; internal set; }
        
        public bool WaitForDeath { get; set; }
        
        public int DeathDelay { get; } = 1000;
        
        #region Persistent Unit Data
        
        public bool IsUnitDataPersistent { get; set; } = false;

        private readonly List<PlayerUnitData> savedUnitData = new List<PlayerUnitData>();
        
        // Called by UnitController after playerUnits are ready. Could also be called by listening
        // to the UnitsReadyCommand<PlayerUnitData> but would risk a race condition.
        public void ImportData()
        {
            if (!IsUnitDataPersistent)
                return;
            
            foreach (var unitData in savedUnitData)
            {
                var unitInScene = Units.FirstOrDefault(unit => unit.Name == unitData.Name);

                // TODO: Could skip this check if Units was a list of PlayerUnits.
                if (unitInScene is PlayerUnit playerUnitInScene)
                    playerUnitInScene.ImportData(unitData);
                else
                    Debug.LogWarning($"{nameof(PlayerManager)} contains non-{nameof(PlayerUnit)}s.");
            }
        }

        // Called by the GameManager when the encounter ends. Could also be called by the
        // TurnManager, but this way makes it possible to test with the NoRemainingEnemyUnitsCommand
        public void ExportData()
        {
            if (!IsUnitDataPersistent)
                return;
            
            savedUnitData.Clear();
            
            foreach (var unit in Units)
            {
                // TODO: Could skip this check if Units was a list of PlayerUnits.
                if (unit is PlayerUnit playerUnit) 
                    savedUnitData.Add(playerUnit.ExportData());
                else
                    Debug.LogWarning($"{nameof(PlayerManager)} contains non-{nameof(PlayerUnit)}s.");
            }
        }
        
        #endregion
    }
}
