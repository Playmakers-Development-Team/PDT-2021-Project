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

        public PlayerUnitData ExportData() => new PlayerUnitData(data);

        public void ImportData(PlayerUnitData playerUnitData)
        {
            // TODO temporarily keep health between levels.
            // TODO Also this is currently broken, we can't just copy it because HealthValue keeps reference to the unit from the last scene, not the new one
            // HealthStat = playerUnitData.HealthValue;
            Abilities = playerUnitData.Abilities;
        }
    }
}
