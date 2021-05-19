using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Manager
    {
        private readonly List<Unit> playerUnits = new List<Unit>();

        public IReadOnlyList<Unit> PlayerUnits => playerUnits.AsReadOnly();
        public int Count => playerUnits.Count;
        
        public Unit Spawn(GameObject playerPrefab, Vector2Int gridPosition)
        {
            Unit unit = UnitUtility.Spawn(playerPrefab, gridPosition);
            
            if (!(unit is PlayerUnitController))
                return null;
            
            playerUnits.Add(unit);
            
            return unit;
        }
        
        public Unit Spawn(string playerName, Vector2Int gridPosition)
        {
            Unit unit = UnitUtility.Spawn(playerName, gridPosition);

            if (!(unit is PlayerUnitController))
                return null;
            
            playerUnits.Add(unit);
            
            return unit;
        }
        
        public void Clear()
        {
            playerUnits.Clear();
        }

        public void Clean()
        {
            for (int i = playerUnits.Count; i >= 0; i--)
            {
                if (playerUnits[i] is null)
                    playerUnits.RemoveAt(i);
            }
        }
    }
}
