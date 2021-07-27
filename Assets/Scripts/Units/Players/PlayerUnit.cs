using Abilities;
using Managers;
using Units.Commands;

namespace Units.Players
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }

        private PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();

            playerManager = ManagerLocator.Get<PlayerManager>();
            
            commandManager.ListenCommand<PlayerManagerReadyCommand>(cmd => Spawn());
        }

        public override bool IsSameTeamWith(IAbilityUser other) => other is PlayerUnit;

        protected override void Spawn() => playerManager.Spawn(this);
    }
}
