using Commands;
using Cysharp.Threading.Tasks;
using Managers;
using Turn;
using Turn.Commands;
using Units.Commands;
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
        }

        private void OnEnable() => commandManager.ListenCommand<StartTurnCommand>(StartTurn);

        private void OnDisable() => commandManager.UnlistenCommand<StartTurnCommand>(StartTurn);

        public async void StartTurn(StartTurnCommand startTurnCommand)
        {
            // Only execute for this unit
            if (!ReferenceEquals(startTurnCommand.Unit, enemyUnit))
                return;

            if (playerManager.Units.Count <= 0)
            {
                Debug.LogWarning("No players remain, enemy intention is to do nothing");
                return;
            }
                
            await DecideEnemyIntention();
            commandManager.ExecuteCommand(new EnemyActionsCompletedCommand(enemyUnit));
        }

        protected abstract UniTask DecideEnemyIntention();
    }
}
