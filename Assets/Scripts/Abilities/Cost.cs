using System;
using StatusEffects;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class Cost
    {
        [SerializeField, HideInInspector] private string name;
        [SerializeField] private AffectType affectType;
        [SerializeField] private CostType costType;
        [SerializeField] private TenetType tenetType;

        public CostType CostType => costType;
        public TenetType TenetType => tenetType;

        public int CalculateBonusMultiplier(IUnit user, IUnit target)
        {
            IUnit unit = GetAffectedUnit(user, target);
            
            if (costType == CostType.Per)
                return unit.GetTenetStatusCount(tenetType);
            
            return 0;
        }

        public void Expend(IUnit user, IUnit target)
        {
            IUnit unit = GetAffectedUnit(user, target);
            
            switch (costType)
            {
                case CostType.Per:
                    unit.RemoveTenetStatus(tenetType);
                    break;
                case CostType.Spend:
                    unit.RemoveTenetStatus(tenetType, 1);
                    break;
            }
        }

        public bool MeetsRequirements(IUnit user, IUnit target) =>
            affectType == AffectType.Target
                ? target.GetTenetStatusCount(tenetType) > 0
                : user.GetTenetStatusCount(tenetType) > 0;

        private IUnit GetAffectedUnit(IUnit user, IUnit target) => 
            affectType == AffectType.Target ? target : user;
    }
}
