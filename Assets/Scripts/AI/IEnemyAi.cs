using Commands;
using Turn;
using Turn.Commands;
using Units.Enemies;
using Units.Players;

namespace AI
{
    public interface IEnemyAi
    {
        public EnemyUnit enemyUnit { get; }
        
        public PlayerManager playerManager { get; }
        public CommandManager commandManager { get; }
        public TurnManager turnManager { get; }
        public EnemyManager enemyManager { get; }

        public int SpecialMoveCount { get; }

        public abstract void StartTurn(StartTurnCommand startTurnCommand);
    }
}
