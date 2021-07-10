using Abilities;
using Managers;
using Units.Commands;
using UnityEngine;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }
        
        private CommandManager commandManager;
        
        protected override void Start()
        {
            base.Start();
            commandManager = ManagerLocator.Get<CommandManager>();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
        }

        
    }
}
