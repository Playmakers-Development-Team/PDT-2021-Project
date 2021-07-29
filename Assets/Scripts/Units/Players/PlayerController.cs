using Commands;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units.Players
{
    public class PlayerController : MonoBehaviour
    {
        private CommandManager commandManager;

        private void Awake()
        {
            #region GetManagers

            commandManager = ManagerLocator.Get<CommandManager>();

            #endregion
        }

        private void Start()
        {
            // Process stuff about players here
            
            commandManager.ExecuteCommand(new PlayerUnitsReadyCommand());
        }
    }
}
