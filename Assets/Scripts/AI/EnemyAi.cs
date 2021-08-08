using Commands;
using Managers;
using Turn;
using Turn.Commands;
using Units.Enemies;
using Units.Players;
using UnityEngine;

namespace AI
{
    public abstract class EnemyAi : MonoBehaviour, IEnemyAi
    {
        public EnemyUnit enemyUnit { get; private set; }
        
        public PlayerManager playerManager { get; private set; }
        public CommandManager commandManager { get; private set; }
        public TurnManager turnManager { get; private set; }
        public EnemyManager enemyManager { get; private set; }
        
        [field: SerializeField] public int SpecialMoveCount { get; set; }
        
        private void Awake()
        {
            enemyUnit = GetComponent<EnemyUnit>();
            
            playerManager = ManagerLocator.Get<PlayerManager>();
            commandManager = ManagerLocator.Get<CommandManager>();
            turnManager = ManagerLocator.Get<TurnManager>();
            enemyManager = ManagerLocator.Get<EnemyManager>();
            
            commandManager.ListenCommand<StartTurnCommand>(StartTurn);
        }
        
        public void StartTurn(StartTurnCommand startTurnCommand)
        {
            if(ReferenceEquals(startTurnCommand.Unit, enemyUnit))
                DecideEnemyIntention();
        }

        protected abstract void DecideEnemyIntention();
    }
}
