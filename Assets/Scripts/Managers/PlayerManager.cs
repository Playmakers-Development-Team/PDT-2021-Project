using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Manager
    {
        public IUnit SelectedUnit { get; private set; }
        private readonly List<IUnit> playerUnits = new List<IUnit>();

        public IReadOnlyList<IUnit> PlayerUnits => playerUnits.AsReadOnly();
        public int Count => playerUnits.Count;
        
        public IUnit Spawn(GameObject playerPrefab, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(playerPrefab, gridPosition);
            
            if (!(unit is PlayerUnit))
                return null;
            
            playerUnits.Add(unit);
            SelectUnit((PlayerUnit)unit);
            return unit;
        }
        
        public IUnit Spawn(string playerName, Vector2Int gridPosition)
        {
            IUnit unit = UnitUtility.Spawn(playerName, gridPosition);

            if (!(unit is PlayerUnit))
                return null;
            
            playerUnits.Add(unit);
            SelectUnit((PlayerUnit)unit);
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

        public void SelectUnit(PlayerUnit unit)
        {
            //if((Object)selectedUnit != unit) ManagerLocator.Get<CommandManager>().QueueCommand(new Commands.UnitSelectedCommand(unit)); //Update UI
            SelectedUnit = unit;
            //Debug.Log(unit + " selected!");
        }
    }
}
