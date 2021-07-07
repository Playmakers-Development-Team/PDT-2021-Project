using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Conditionals
{
    [Serializable]
    public class Bonus : Conditional
    {
        [SerializeField] private TenetType[] perTenet;
        
        public int CalculateBonusMultiplier(IUnit user, IUnit target)
        {
            IUnit unit = GetAffectedUnit(user, target);
            int bonus = 0;
            
            foreach (TenetType tenetType in perTenet)
            {
                bonus += unit.GetTenetStatusCount(tenetType);
            }

            return bonus;
        }
    }
}
