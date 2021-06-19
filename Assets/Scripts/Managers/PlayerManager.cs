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
            return UnitUtility.Spawn(playerPrefab, gridPosition);
        }
        
        public IUnit Spawn(string playerName, Vector2Int gridPosition)
        {
            return UnitUtility.Spawn(playerName, gridPosition);
        }

        public IUnit Spawn(IUnit unit)
        {
            if (!(unit is PlayerUnit))
                return null;
            
            playerUnits.Add(unit);
            
            ManagerLocator.Get<TurnManager>().AddNewUnitToTimeline(unit);
            
            SelectUnit((PlayerUnit)unit);
            
            return unit;
        }
        
        public void Clear()
        {
            playerUnits.Clear();
        }

        public void ClearUnits()
        {
            for (int i = playerUnits.Count; i >= 0; i--)
            {
                if (playerUnits[i] is null)
                    playerUnits.RemoveAt(i);
            }
        }

        public void SelectUnit(PlayerUnit unit)
        {
            if (unit is null)
            {
                Debug.LogWarning("PlayerManager.SelectUnit should not be passed a null value. Use PlayerManager.DeselectUnit instead.");
                DeselectUnit();
                return;
            }
            
            if ((PlayerUnit) SelectedUnit != unit)
            {
                SelectedUnit = unit;
                
                ManagerLocator.Get<CommandManager>().
                    ExecuteCommand(new Commands.UnitSelectedCommand(unit));
                
                Debug.Log(unit + " selected!");
            }
        }

        public void DeselectUnit()
        {
            SelectedUnit = null;
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.UnitDeselectedCommand(SelectedUnit));
            // Debug.Log("Units deselected.");
        }
        
        public void RemovePlayerUnit(IUnit playerUnit)
        {
            if (playerUnits.Contains(playerUnit))
            {
                playerUnits.Remove(playerUnit);
                Debug.Log(playerUnits.Count + " players remain");
            }
            else
            {
                Debug.LogWarning("WARNING: Tried to remove " + playerUnit +
                                 " from PlayerManager but it isn't a part of the playerUnits list");
            }
        }
    }
}
