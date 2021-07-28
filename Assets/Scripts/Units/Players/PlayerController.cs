using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Players
{
    public class PlayerController : UnitController<PlayerUnitData>
    {
        protected override void Awake()
        {
            base.Awake();
            
            #region GetManagers

            unitManagerT = ManagerLocator.Get<PlayerManager>();

            #endregion
            
            commandManager.ListenCommand<StatChangedCommand>(cmd =>
            {
                if (cmd.Unit is null)
                    return;
                
                Debug.Log($"{cmd.Unit.Name} , {cmd.StatType} has changed by {cmd.Value}");
            });
        }
    }
}
