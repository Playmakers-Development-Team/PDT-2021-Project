using System.Collections.Generic;
using System.Linq;

namespace Abilities.Parsing
{
    internal class EffectParser
    {
        private readonly IAbilityContext abilityContext;
        private readonly IReadOnlyList<Effect> effects;
        private readonly HashSet<Effect> spentUserEffects = new HashSet<Effect>();

        public IAbilityUser User => abilityContext.OriginalUser;

        public EffectParser(IAbilityContext abilityContext, EffectOrder effectOrder, IEnumerable<Effect> effects)
        {
            this.effects = effects
                .Where(e => e.EffectOrder == effectOrder)
                .ToList()
                .AsReadOnly();
            this.abilityContext = abilityContext;
        }
        
        public void Parse(IAbilityUser target)
        {
            int attack = CalculateValue(target, EffectValueType.Attack);
            int defence = CalculateValue(target, EffectValueType.Defence);
            int damage = CalculateValue(target, EffectValueType.Damage);
            int directDamage = CalculateValue(target, EffectValueType.DirectDamage);
            int attackForEncounter = CalculateValue(target, EffectValueType.AttackForEncounter);
            int defenceForEncounter = CalculateValue(target, EffectValueType.DefenceForEncounter);
            
            target.TakeAttack(attack);
            target.TakeDefence(defence);
            target.TakeDamage(directDamage);
            User.DealDamageTo(target, damage);
                
            target.TakeAttackForEncounter(attackForEncounter);
            target.TakeDefenceForEncounter(defenceForEncounter);

            // Check if knockback is supported first, because currently it sometimes doesn't
            //if (target.Knockback != null)
            //target.TakeKnockback(knockback);
                
            foreach (Effect effect in effects)
            {
                if (effect.CanBeUsedWith(abilityContext, User, target))
                {
                    effect.ProvideTenet(target);
                    effect.ApplyTargetCosts(abilityContext, target);
                    // Keep track of the effect, so that we can apply costs towards user later
                    // We can't simply apply costs towards the user here because then it will be applied as many
                    // as there are targets, rather it should only be applied once to the user.
                    spentUserEffects.Add(effect);
                }
            }
        }

        /// <summary>
        /// <p>All user costs that has been parsed is applied.</p>
        /// 
        /// <p>We want to apply all costs towards the user only once, so do call this function right after
        /// parsing all the targets.</p>
        /// </summary>
        public void ApplyUserCosts()
        {
            foreach (Effect effect in spentUserEffects)
                effect.ApplyUserCosts(abilityContext, User);
        }

        /// <summary>
        /// Sum up all the values from each effect. Only count the effect if such effect can be used.
        /// </summary>
        private int CalculateValue(IAbilityUser target, EffectValueType valueType) =>
            effects.Where(e => e.CanBeUsedWith(abilityContext, User, target))
                .Sum(e => e.CalculateValue(abilityContext, target, valueType));
    }
}
