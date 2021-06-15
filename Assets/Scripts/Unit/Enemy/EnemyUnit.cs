using System.Collections.Generic;

namespace Unit.Enemy
{
    public class EnemyUnit : Unit<EnemyUnitData>
    {
        private List<Command.Command> commandQueue = new List<Command.Command>();

        public void QueueCommand(Command.Command command)
        {
            commandQueue.Add(command);
        }
        
        public void ExecuteQueue () {
            foreach (var command in commandQueue) {
                command.Execute();
            }

            commandQueue.Clear();
        }   
    }
}
