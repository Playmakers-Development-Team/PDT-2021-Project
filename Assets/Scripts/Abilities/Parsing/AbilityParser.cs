using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Abilities.Parsing
{
    internal class AbilityParser
    {
        private readonly AbilityContextHandler abilityContextHandler;
        private readonly IVirtualAbilityUser user;
        private readonly ICollection<Effect> targetEffects;
        private readonly ICollection<Effect> userEffects;
        private readonly ICollection<IVirtualAbilityUser> targets;
        
        public AbilityParser(IAbilityUser user, ICollection<Effect> effects, IEnumerable<IAbilityUser> targets)
        {
            this.user = user.CreateVirtualAbilityUser();
            this.targetEffects = effects
                .Where(e => e.affectTargets)
                .ToList();
            this.userEffects = effects
                .Where(e => e.affectUser)
                .ToList();
            this.targets = targets
                .Select(t => t.CreateVirtualAbilityUser())
                .ToList();
            this.abilityContextHandler = new AbilityContextHandler(this.user, this.targets.Append(this.user));
        }

        public void ParseAll()
        {
            foreach (EffectOrder effectOrder in Enum.GetValues(typeof(EffectOrder)))
                ParseOrder(effectOrder);
        }

        public void UndoAll()
        {
            foreach (EffectOrder effectOrder in Enum.GetValues(typeof(EffectOrder)))
                UndoOrder(effectOrder);
        }

        public void ApplyChanges()
        {
            abilityContextHandler.ApplyChanges();
        }

        private void ParseOrder(EffectOrder effectOrder)
        {
            EffectParser userEffectsParser = new EffectParser(abilityContextHandler, effectOrder, userEffects);
            EffectParser targetEffectsParser = new EffectParser(abilityContextHandler, effectOrder, targetEffects);

            foreach (IVirtualAbilityUser target in targets)
            {
                if (target != user)
                    targetEffectsParser.Parse(target);
            }
            
            userEffectsParser.Parse(user);
            ApplyUserCosts();
        }
        
        /// <summary>
        /// Apply all the costs from the effect affecting the user. This function needs to be applied
        /// after effects are applied.
        /// The same Keywords will then not be applied more than once.
        /// </summary>
        public void ApplyUserCosts()
        {
            HashSet<Keyword> visitedKeywords = new HashSet<Keyword>();
            
            foreach (Effect effect in userEffects)
            {
                if (effect.CanBeUsedByUser(abilityContextHandler, user))
                {
                    effect.ApplyEffectCosts(abilityContextHandler, user, false);

                    foreach (Keyword keyword in effect.Keywords)
                    {
                        // The same keyword cost anywhere affecting the user will not be applied more than once.
                        if (!visitedKeywords.Contains(keyword))
                        {
                            visitedKeywords.Add(keyword);
                            keyword.Effect.ApplyCombinedCosts(abilityContextHandler, user, false);
                        }
                    }
                }
            }
        }

        private void UndoOrder(EffectOrder effectOrder)
        {
            throw new NotImplementedException();
        }
    }
}
