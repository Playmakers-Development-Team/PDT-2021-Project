using System;
using System.Linq;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities.Costs
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private AffectType affectType;
        [SerializeField] private TenetCost[] tenetCosts;

        public int CalculateBonusMultiplier(IUnit user, IUnit target)
        {
            IUnit unit = GetAffectedUnit(user, target);

            foreach (TenetCost tenetCost in tenetCosts)
            {
                if (tenetCost.TenetCostType == TenetCostType.Per)
                    return unit.GetTenetStatusCount(tenetCost.TenetType);
            }

            return 0;
        }

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

        private IUnit GetAffectedUnit(IUnit user, IUnit target) => 
            affectType == AffectType.Target ? target : user;
    }
}
