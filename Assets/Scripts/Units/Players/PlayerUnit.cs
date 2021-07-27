using Abilities;
using Managers;
using Units.Commands;

namespace Units.Players
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }

        private PlayerManager playerManager;
        
        public override bool IsSameTeamWith(IAbilityUser other) => other is PlayerUnit;

        protected override void Awake()
        {
            base.Awake();

            playerManager = ManagerLocator.Get<PlayerManager>();
        }

        private void OnEnable() =>
            commandManager.ListenCommand<PlayerManagerReadyCommand>(OnPlayerManagerReady);
        
        private void OnDisable() =>
            commandManager.UnlistenCommand<PlayerManagerReadyCommand>(OnPlayerManagerReady);

        private void OnPlayerManagerReady(PlayerManagerReadyCommand cmd) => Spawn();

        protected override void Spawn() => playerManager.Spawn(this);
    }
}
