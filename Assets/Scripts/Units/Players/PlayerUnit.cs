using Abilities;
using Managers;

namespace Units.Players
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility { get; set; }
        
        protected override void Start()
        {
            base.Start();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
        }
    }
}
