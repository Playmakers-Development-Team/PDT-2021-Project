using System.Collections.Generic;
using System.Linq;

namespace Abilities.Parsing
{
    internal class EffectParser
    {
        private readonly IAbilityUser user;
        private readonly IReadOnlyList<Effect> effects;

        public EffectParser(IAbilityUser user, EffectOrder effectOrder, IEnumerable<Effect> effects)
        {
            this.user = user;
            this.effects = effects
                .Where(e => e.EffectOrder == effectOrder)
                .ToList()
                .AsReadOnly();
        }
        
        public void Process(IAbilityUser abilityUser)
        {
            int attack = CalculateValue(abilityUser, EffectValueType.Attack);
            int defence = CalculateValue(abilityUser, EffectValueType.Defence);
            int damage = CalculateValue(abilityUser, EffectValueType.Damage);
            int directDamage = CalculateValue(abilityUser, EffectValueType.DirectDamage);
            int attackForEncounter = CalculateValue(abilityUser, EffectValueType.AttackForEncounter);
            int defenceForEncounter = CalculateValue(abilityUser, EffectValueType.DefenceForEncounter);
            
            abilityUser.TakeAttack(attack);
            abilityUser.TakeDefence(defence);
            abilityUser.TakeDamage(directDamage);
            user.DealDamageTo(abilityUser, damage);
                
            abilityUser.TakeAttackForEncounter(attackForEncounter);
            abilityUser.TakeDefenceForEncounter(defenceForEncounter);

            // Check if knockback is supported first, because currently it sometimes doesn't
            //if (target.Knockback != null)
            //target.TakeKnockback(knockback);
                
            foreach (Effect effect in effects)
            {
                if (effect.CanBeUsedWith(user, abilityUser))
                    effect.ProvideTenet(abilityUser);
                
                if (effect.CanBeUsedForTarget(abilityUser))
                    effect.ApplyCombinedCosts(user, true);
            }
        }

        /// <summary>
        /// Sum up all the values from each effect. Only count the effect if such effect can be used.
        /// </summary>
        private int CalculateValue(IAbilityUser target, EffectValueType valueType) =>
            effects.Where(e => e.CanBeUsedWith(user, target))
                .Sum(e => e.CalculateValue(user, target, valueType));
    }
}
