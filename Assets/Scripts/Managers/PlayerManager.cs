using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Random = UnityEngine.Random;

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
            return Spawn(UnitUtility.Spawn(playerPrefab, gridPosition));
        }
        
        public IUnit Spawn(string playerName, Vector2Int gridPosition)
        {
            return Spawn(UnitUtility.Spawn(playerName, gridPosition));
        }

        public IUnit Spawn(IUnit unit)
        {
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
            if ((PlayerUnit) SelectedUnit != unit)
            {
                ManagerLocator.Get<CommandManager>().
                    QueueCommand(new Commands.UnitSelectedCommand(unit));
                
                SelectedUnit = unit;
                
                // Debug.Log(unit + " selected!");
            }
        }

        public void DeselectUnit()
        {
            SelectedUnit = null;
            // Debug.Log("Units deselected.");
        }
        
        public void RemovePlayerUnit(IUnit playerUnit)
        {
            if (playerUnits.Contains(playerUnit))
            {
                playerUnits.Remove(playerUnit);
                Debug.Log(playerUnits.Count + " enemies remain");
            }
            else
            {
                Debug.LogWarning("WARNING: Tried to remove " + playerUnit +
                                 " from PlayerManager but it isn't a part of the playerUnits list");
            }
        }
    }
}
