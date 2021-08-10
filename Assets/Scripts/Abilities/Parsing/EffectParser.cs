using System.Collections.Generic;
using System.Linq;

namespace Abilities.Parsing
{
    internal class EffectParser
    {
        private readonly IAbilityContext abilityContext;
        private readonly IReadOnlyList<Effect> effects;

        public IAbilityUser User => abilityContext.OriginalUser;

        public EffectParser(IAbilityContext abilityContext, EffectOrder effectOrder, IEnumerable<Effect> effects)
        {
            this.effects = effects
                .Where(e => e.EffectOrder == effectOrder)
                .ToList()
                .AsReadOnly();
            this.abilityContext = abilityContext;
        }
        
        public void Parse(IAbilityUser abilityUser)
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
            User.DealDamageTo(abilityUser, damage);
                
            abilityUser.TakeAttackForEncounter(attackForEncounter);
            abilityUser.TakeDefenceForEncounter(defenceForEncounter);

            // Check if knockback is supported first, because currently it sometimes doesn't
            //if (target.Knockback != null)
            //target.TakeKnockback(knockback);
                
            foreach (Effect effect in effects)
            {
                if (effect.CanBeUsedWith(abilityContext, User, abilityUser))
                    effect.ProvideTenet(abilityUser);
                
                if (effect.CanBeUsedForTarget(abilityContext, abilityUser))
                    effect.ApplyCombinedCosts(abilityContext, User, true);
            }
        }

        /// <summary>
        /// Sum up all the values from each effect. Only count the effect if such effect can be used.
        /// </summary>
        private int CalculateValue(IAbilityUser target, EffectValueType valueType) =>
            effects.Where(e => e.CanBeUsedWith(abilityContext, User, target))
                .Sum(e => e.CalculateValue(abilityContext, target, valueType));
    }
}
