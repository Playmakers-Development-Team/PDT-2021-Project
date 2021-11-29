using Abilities;
using Managers;
using Units.Stats;

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

        public PlayerUnitData ExportData() => new PlayerUnitData(data);

        public void ImportData(PlayerUnitData playerUnitData)
        {
            // We need to specifically copy the value here because HealthValue keeps reference to the unit from the last scene, not the new one for some reason
            // Comment this out if we want to disable health
            HealthStat.Value = playerUnitData.HealthValue.Value;
            Abilities = playerUnitData.Abilities;
        }
    }
}
