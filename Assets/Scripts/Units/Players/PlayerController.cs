using Grid.Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Players
{
    public class PlayerController : UnitController<PlayerUnitData>
    {
        [SerializeField] private bool persistentUnitData = false;

        private PlayerManager PlayerManager => (PlayerManager) unitManagerT;
        
        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            unitManagerT = ManagerLocator.Get<PlayerManager>();

            #endregion

            PlayerManager.IsUnitDataPersistent = persistentUnitData;
        }
        
        // TODO: Repeated code. See UnitController.OnGridObjectsReady.
        protected override void OnGridObjectsReady(GridObjectsReadyCommand cmd)
        {
            unitManagerT.ClearUnits();
            
            commandManager.ExecuteCommand(new UnitManagerReadyCommand<PlayerUnitData>());

            if (unitManagerT is PlayerManager playerManager)
                playerManager.ImportData();
            
            commandManager.ExecuteCommand(new UnitsReadyCommand<PlayerUnitData>());
        }
    }
}
