using Abilities;
using Managers;

namespace Units.Players
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }

        public override bool IsSameTeamWith(IAbilityUser other) => other is PlayerUnit;

        protected override void Awake()
        {
            base.Awake();

            unitManagerT = ManagerLocator.Get<PlayerManager>();
        }
    }
}
