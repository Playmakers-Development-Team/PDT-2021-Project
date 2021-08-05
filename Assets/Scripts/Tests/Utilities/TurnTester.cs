using System.Collections;
using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Turn;
using Turn.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;

namespace Tests.Utilities
{
    public static class TurnTester
    {
        private static CommandManager CommandManager => ManagerLocator.Get<CommandManager>();
        private static TurnManager TurnManager => ManagerLocator.Get<TurnManager>();

        public static IEnumerator WaitPlayerTurn()
        {
            yield return new WaitUntil(() => TurnManager.ActingUnit is PlayerUnit);
        }
        
        public static IEnumerator WaitEnemyTurn()
        {
            yield return new WaitUntil(() => TurnManager.ActingUnit is EnemyUnit);
        }
    }
}
