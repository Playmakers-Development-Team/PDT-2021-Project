using System.Collections.Generic;
using Abilities;
using Commands;
using Managers;
using Units.Players;
using UnityEngine;

namespace Units.Enemies
{
    public class EnemyUnit : Unit<EnemyUnitData>
    {
        [SerializeField] private EnemyType enemyType;
        
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
            //Name = RandomizeName();
        }

        public override bool IsSameTeamWith(IAbilityUser other) => other is EnemyUnit;
    }
}
