using System.Collections.Generic;
using Units;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Manager
    {
        public IUnit SelectedUnit { get; private set; }
        
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
                    ExecuteCommand(new Commands.UnitSelectedCommand(SelectedUnit));
            }
        }

        public void DeselectUnit()
        {
            SelectedUnit = null;
            ManagerLocator.Get<CommandManager>().
                ExecuteCommand(new Commands.UnitDeselectedCommand(SelectedUnit));
        }
        
      
    }
}
