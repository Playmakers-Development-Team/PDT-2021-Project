using System.Collections.Generic;
using Commands;
using Managers;

namespace Units
{
    public class EnemyUnit : Unit<EnemyUnitData>
    {
        public PlayerUnit Target;

        private List<Command> commandQueue = new List<Command>();

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
        
        protected override void Start()
        {
            base.Start();
            ManagerLocator.Get<EnemyManager>().Spawn(this);
            Name = RandomizeName();
        }

        public override bool IsSameTeamWith(IUnit other) => other is EnemyUnit;
    }
}
