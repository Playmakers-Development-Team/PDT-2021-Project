using Unit.Abilities;

namespace Unit.Player
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility;
    }
}
