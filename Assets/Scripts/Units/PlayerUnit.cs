using System;
using Abilities;
using Managers;

namespace Units
{
    public class PlayerUnit : Unit<PlayerUnitData>
    {
        protected override void Start()
        {
            base.Start();
            ManagerLocator.Get<PlayerManager>().Spawn(this);
        }
    }
}
