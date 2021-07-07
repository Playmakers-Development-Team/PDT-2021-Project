using System;
using System.Linq;
using Units;
using UnityEngine;

namespace Abilities.Conditionals
{
    [Serializable]
    public class Cost : Conditional
    {
        [SerializeField] protected TenetCost[] tenetCosts;
        
        public void ApplyCost(IUnit user, IUnit target)
        {
            IUnit unit = GetAffectedUnit(user, target);

            foreach (TenetCost tenetCost in tenetCosts)
            {
                tenetCost.ExpendTenet(unit);
            }
        }

        public bool MeetsRequirements(IUnit user, IUnit target) =>
            MeetsTenetRequirements(user, target);

        public bool MeetsTenetRequirements(IUnit user, IUnit target) =>
            tenetCosts.All(t =>
                affectType == AffectType.Target
                    ? target.GetTenetStatusCount(t.TenetType) > 0
                    : user.GetTenetStatusCount(t.TenetType) > 0);
    }
}
