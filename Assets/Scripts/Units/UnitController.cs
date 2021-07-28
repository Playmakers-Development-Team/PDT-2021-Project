using Commands;
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

        protected virtual void Start()
        {
            unitManagerT.ClearUnits();
            
            commandManager.ExecuteCommand(new UnitManagerReadyCommand<T>());
            
            commandManager.ExecuteCommand(new UnitsReadyCommand<T>());
        }
    }
}
