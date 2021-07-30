using System;
using Commands;
using Grid.Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units
{
    public abstract class UnitController<T> : MonoBehaviour where T : UnitData
    {
        protected UnitManager<T> unitManagerT;
        protected CommandManager commandManager;

        protected virtual void Awake()
        {
            #region GetManagers

            commandManager = ManagerLocator.Get<CommandManager>();

            #endregion
        }

        private void OnEnable() => commandManager.ListenCommand<GridReadyCommand>(OnGridReady);

        private void OnDisable() => commandManager.UnlistenCommand<GridReadyCommand>(OnGridReady);

        private void OnGridReady(GridReadyCommand obj)
        {
            unitManagerT.ClearUnits();
            
            commandManager.ExecuteCommand(new UnitManagerReadyCommand<T>());
            
            commandManager.ExecuteCommand(new UnitsReadyCommand<T>());
        }
    }
}
