using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using Units.Commands;
using Units.Players;

namespace Units.Enemies
{
    public class EnemyUnit : Unit<EnemyUnitData>
    {
        public PlayerUnit Target;

        private List<Command> commandQueue = new List<Command>();
        
        private EnemyManager enemyManager;

        public void QueueCommand(Command command)
        {
            commandQueue.Add(command);
        }
        
        public void ExecuteQueue() 
        {
            foreach (var command in commandQueue)
                command.Execute();

            commandQueue.Clear();
        }   
        
        public override bool IsSameTeamWith(IAbilityUser other) => other is EnemyUnit;
        
        protected override void Awake()
        {
            base.Awake();
            
            enemyManager = ManagerLocator.Get<EnemyManager>();
            
            //Name = RandomizeName();
        }

        private void OnEnable() =>
            commandManager.ListenCommand<EnemyManagerReadyCommand>(OnEnemyManagerReady);
        
        private void OnDisable() =>
            commandManager.UnlistenCommand<EnemyManagerReadyCommand>(OnEnemyManagerReady);

        private void OnEnemyManagerReady(EnemyManagerReadyCommand cmd) => Spawn();
        
        protected override void Spawn() => enemyManager.Spawn(this);
    }
}
