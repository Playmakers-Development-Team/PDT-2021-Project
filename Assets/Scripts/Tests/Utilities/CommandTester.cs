using Commands;
using Managers;
using Turn;
using Turn.Commands;

namespace Tests.Utilities
{
    public static class CommandTester
    {
        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        private static TurnManager TurnManager => ManagerLocator.Get<TurnManager>();

        public static void EndCurrentUnitTurn()
        {
            CommandManager.ExecuteCommand(new EndTurnCommand(TurnManager.ActingUnit));
        }
    }
}
