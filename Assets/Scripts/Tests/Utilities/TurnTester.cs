using System.Collections;
using Commands;
using Managers;
using Turn;
using Turn.Commands;
using Units.Enemies;
using Units.Players;

namespace Tests.Utilities
{
    public static class TurnTester
    {
        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        private static TurnManager TurnManager => ManagerLocator.Get<TurnManager>();

        public static IEnumerator WaitPlayerTurn()
        {
            yield return CommandManager
                .WaitForCommandYield<StartTurnCommand>(cmd => cmd.Unit is PlayerUnit);
        }
        
        public static IEnumerator WaitEnemyTurn()
        {
            yield return CommandManager
                .WaitForCommandYield<StartTurnCommand>(cmd => cmd.Unit is EnemyUnit);
        }
    }
}
