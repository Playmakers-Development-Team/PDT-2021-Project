using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject
    {
        [SerializeField] private int knockback;
        
        [SerializeField] private Effect[] targetEffects;
        [SerializeField] private Effect[] userEffects;
        

        public void Use(IUnit user, IUnit target)
        {
            Use(user, target, targetEffects);
            Use(user, user, userEffects);
        }

        private void Use(IUnit user, IUnit target, Effect[] effects)
        {
            int damage = CalculateAmount(user, effects, EffectValueType.Damage);
            int defence = CalculateAmount(user, effects, EffectValueType.Defence);
            int attack = CalculateAmount(user, effects, EffectValueType.Attack);

            target.TakeDamage(damage);
            target.AddAttack(attack);
            target.AddDefence(defence);
            
            // TODO: Knockback??
        }

        private int CalculateAmount(IUnit user, Effect[] effects, EffectValueType valueType)
        {
            int bonus = 0;
            
            foreach (Effect effect in effects)
            {
                if (!effect.CanUse(user))
                    continue;

                bonus += effect.CalculateModifier(user, valueType);
                effect.Expend(user);
            }

            return bonus;
        }
    }
}
