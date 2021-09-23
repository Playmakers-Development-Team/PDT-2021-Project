using Abilities;
using Grid.Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Players
{
    public class PlayerController : UnitController<PlayerUnitData>
    {
        [Tooltip("The ability pool which will be used to pickup new ability at the end of encounter")]
        [SerializeField] private AbilityPool abilityPickupPool;
        [SerializeField] private bool persistentUnitData = false;

        private PlayerManager PlayerManager => (PlayerManager) unitManagerT;
        
        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            unitManagerT = ManagerLocator.Get<PlayerManager>();

            #endregion

            PlayerManager.IsUnitDataPersistent = persistentUnitData;
            PlayerManager.AbilityPickupPool = abilityPickupPool;
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
