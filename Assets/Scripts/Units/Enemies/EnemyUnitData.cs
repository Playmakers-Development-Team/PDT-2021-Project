using System;
using UnityEngine;

namespace Units.Enemies
{
    [Serializable]
    public class EnemyUnitData : UnitData
    {
        // Put enemy-specific stats here
        public int TemporaryAttackIncrease;
        public int TemporaryDefenceIncrease;
        public int PermanentAttackIncrease;
    }
}
