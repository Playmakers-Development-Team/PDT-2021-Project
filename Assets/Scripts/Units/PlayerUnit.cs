using System;
using Abilities;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        public Ability CurrentlySelectedAbility;
    }
}
