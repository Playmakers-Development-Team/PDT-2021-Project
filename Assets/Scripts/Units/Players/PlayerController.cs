using Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Players
{
    public class PlayerController : MonoBehaviour
    {
        private CommandManager commandManager;
        private PlayerManager playerManager;

        private void Awake()
        {
            #region GetManagers

            commandManager = ManagerLocator.Get<CommandManager>();
            playerManager = ManagerLocator.Get<PlayerManager>();

            #endregion
            
            commandManager.ListenCommand<StatChangedCommand>(cmd =>
            {
                if (cmd.Unit is null)
                    return;
                
                Debug.Log($"{cmd.Unit.Name} , {cmd.StatType} has changed by {cmd.Value}");
            });
        }

        private void Start()
        {
            playerManager.ClearUnits();
            
            commandManager.ExecuteCommand(new UnitManagerReadyCommand<PlayerUnitData>());
            
            commandManager.ExecuteCommand(new PlayerUnitsReadyCommand());
        }
    }
}
