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

        public IVirtualAbilityUser User => user;
        public ICollection<IVirtualAbilityUser> Targets => targets;
        
        private static readonly EffectOrder[] order =
        {
            EffectOrder.Early,
            EffectOrder.Regular,
            EffectOrder.Late
        };

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
            foreach (EffectOrder effectOrder in order)
                ParseOrder(effectOrder);
        }

        public void UndoAll()
        {
            foreach (EffectOrder effectOrder in order)
                UndoOrder(effectOrder);
        }

        public void ApplyChanges() => abilityContextHandler.ApplyChanges();

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
            userEffectsParser.ApplyUserCosts();
            targetEffectsParser.ApplyUserCosts();
        }

        private void UndoOrder(EffectOrder effectOrder)
        {
            throw new NotImplementedException();
        }
    }
}
