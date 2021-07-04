using System.Collections.Generic;
using Commands;

namespace Units
{
    public class EnemyUnit : Unit<EnemyUnitData>
    {
        private List<Command> commandQueue = new List<Command>();

        public void QueueCommand(Command command)
        {
            commandQueue.Add(command);
        }
        
        public void ExecuteQueue() 
        {
            foreach (var command in commandQueue) 
            {
                command.Execute();
            }

            commandQueue.Clear();
        }   
    }
}
