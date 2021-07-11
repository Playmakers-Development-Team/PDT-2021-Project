using Abilities;
using Managers;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }
        
        protected override void Start()
        {
            base.Start();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
        }

        public override bool IsSameTeamWith(IUnit other) => other is PlayerUnit;
    }
}
