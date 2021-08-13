using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Abilities.Parsing
{
    internal class AbilityParser
    {
        private readonly IAbilityUser user;
        private readonly ICollection<Effect> targetEffects;
        private readonly ICollection<Effect> userEffects;
        private readonly ICollection<IAbilityUser> targets;
        
        public AbilityParser(IAbilityUser user, ICollection<Effect> effects, IEnumerable<IAbilityUser> targets)
        {
            this.user = user;
            this.targetEffects = effects
                .Where(e => e.affectTargets)
                .ToList();
            this.userEffects = effects
                .Where(e => e.affectUser)
                .ToList();
            this.targets = targets.ToList();
        }

        public void ProcessAll()
        {
            foreach (EffectOrder effectOrder in Enum.GetValues(typeof(EffectOrder)))
                ProcessOrder(effectOrder);
        }

        public void UndoAll()
        {
            foreach (EffectOrder effectOrder in Enum.GetValues(typeof(EffectOrder)))
                UndoOrder(effectOrder);
        }
        
        private void ProcessOrder(EffectOrder effectOrder)
        {
            EffectParser userEffectsParser = new EffectParser(user, effectOrder, userEffects);
            EffectParser targetEffectsParser = new EffectParser(user, effectOrder, targetEffects);

            foreach (IAbilityUser target in targets)
            {
                if (target != user)
                    targetEffectsParser.Process(target);
            }
            
            userEffectsParser.Process(user);
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
                if (effect.CanBeUsedByUser(user))
                {
                    effect.ApplyEffectCosts(user, false);

                    foreach (Keyword keyword in effect.Keywords)
                    {
                        // The same keyword cost anywhere affecting the user will not be applied more than once.
                        if (!visitedKeywords.Contains(keyword))
                        {
                            visitedKeywords.Add(keyword);
                            keyword.Effect.ApplyCombinedCosts(user, false);
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
