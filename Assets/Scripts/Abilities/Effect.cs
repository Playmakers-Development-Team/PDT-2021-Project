using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Effect
    {
        [SerializeField] private int damageModifier;
        [SerializeField] private int defenceModifier;
        [SerializeField] private Cost[] costs;

        public bool CanUse(UnitData user)
        {
            UnitData clone = new UnitData(user);
            
            foreach (Cost cost in costs)
            {
                if (!cost.CanAfford(clone))
                    return false;

                cost.Expend(clone);
            }

            return true;
        }

        public void Expend(UnitData user)
        {
            if (!CanUse(user))
                return;

            foreach (Cost cost in costs)
                cost.Expend(user);
        }

        public int CalculateDamageModifier(UnitData user) => throw new NotImplementedException();
        
        public int CalculateDefenceModifier(UnitData user) => throw new NotImplementedException();
    }
}
