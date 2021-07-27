using Abilities;
using Managers;

namespace Units.Players
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
        }

        public override bool IsSameTeamWith(IAbilityUser other) => other is PlayerUnit;
    }
}
