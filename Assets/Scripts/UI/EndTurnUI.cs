using Commands;
using Managers;
using UnityEngine;

namespace UI
{
    public class EndTurnUI : MonoBehaviour
    {
        private CommandManager commandManager;
        private TurnManager turnManager;

        private void Start()
        {
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
        }

        public void EndTurn()
        {
            commandManager.ExecuteCommand(new EndTurnCommand(turnManager.CurrentUnit));
        }
    }
}
