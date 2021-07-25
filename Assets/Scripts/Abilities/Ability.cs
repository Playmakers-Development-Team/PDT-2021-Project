﻿using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Shapes;
using Cysharp.Threading.Tasks;
using Grid.GridObjects;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject
    {
        [Tooltip("Complete description of the ability")]
        [SerializeField, TextArea(4, 8)] private string description;
        [SerializeField] private BasicShapeData shape;
        [SerializeField] private bool excludeUserFromTargets = true;
        // [SerializeField] private int knockback;

        [SerializeField] private Effect[] targetEffects;
        [SerializeField] private Effect[] userEffects;

        /// <summary>
        /// A complete description of the ability.
        /// </summary>
        public string Description => description;
        /// <summary>
        /// Describes what and how the ability can hit units.
        /// </summary>
        public IShape Shape => shape;
        /// <summary>
        /// All keywords used by this ability regardless whether they should be shown to
        /// the player or not.
        /// </summary>
        public IEnumerable<Keyword> AllKeywords => TargetKeywords.Concat(UserKeywords);
        /// <summary>
        /// All keywords that should be shown in game which is used by this ability.
        /// </summary>
        public IEnumerable<Keyword> AllVisibleKeywords => AllKeywords.Where(k => k.IsVisibleInGame);

        private IEnumerable<Keyword> TargetKeywords => targetEffects.SelectMany(e => e.Keywords);
        private IEnumerable<Keyword> UserKeywords => userEffects.SelectMany(e => e.Keywords);

        public void Use(IAbilityUser user, Vector2Int originCoordinate, Vector2 targetVector) =>
            UseForTargets(user, shape.GetTargets(originCoordinate, targetVector));

        public void UseForTargets(IAbilityUser user, params GridObject[] targets) => 
            UseForTargets(user, targets.AsEnumerable());
        
        public void UseForTargets(IAbilityUser user, IEnumerable<GridObject> targets)
        {
            IEnumerable<GridObject> finalTargets = excludeUserFromTargets
                ? targets.Where(u => !ReferenceEquals(user, u))
                : targets;
            UseEffectsOnTargets(user, targetEffects, finalTargets);

            // It can be assumed that IAbilityUser can be converted to GridObject.
            if (user is GridObject userGridObject)
                UseEffectsOnTargets(user, userEffects, userGridObject);
        }

        private void UseEffectsOnTargets(IAbilityUser user, Effect[] effects, params GridObject[] targets) =>
            UseEffectsOnTargets(user, effects, targets.AsEnumerable());

        private void UseEffectsOnTargets(IAbilityUser user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            // Please ignore the multiple enumeration for now, isn't a bad thing since
            // it doesn't actually make a performance difference because of the later code
            UseEffectsOnTargetsForOrder(user, effects, targets, EffectOrder.Early);
            UseEffectsOnTargetsForOrder(user, effects, targets, EffectOrder.Regular);
            UseEffectsOnTargetsForOrder(user, effects, targets, EffectOrder.Late);
        }

        private void UseEffectsOnTargetsForOrder(IAbilityUser user, IEnumerable<Effect> effects,
                                                 IEnumerable<GridObject> targets, 
                                                 EffectOrder effectOrder)
        {
            Effect[] effectsWithOrder = effects
                .Where(e => e.EffectOrder == effectOrder)
                .ToArray();
            
            foreach (GridObject target in targets.ToArray())
            {
                if (target is IAbilityUser targetUnit)
                {
                    int attack = CalculateValue(user, targetUnit, effectsWithOrder, EffectValueType.Attack);
                    int defence = CalculateValue(user, targetUnit, effectsWithOrder, EffectValueType.Defence);
                    int damage = CalculateValue(user, targetUnit, effectsWithOrder, EffectValueType.Damage);
                    int directDamage = CalculateValue(user, targetUnit, effectsWithOrder, EffectValueType.DirectDamage);
                
                    targetUnit.TakeAttack(attack);
                    targetUnit.TakeDefence(defence);
                    targetUnit.TakeDamage(directDamage);
                    user.DealDamageTo(targetUnit, damage);

                    // Check if knockback is supported first, because currently it sometimes doesn't
                    //if (targetUnit.Knockback != null)
                    //targetUnit.TakeKnockback(knockback);
                    
                    foreach (Effect effect in effectsWithOrder)
                    {
                        effect.ProcessTenet(user, targetUnit);
                    }
                }
            }
        }
        
        public void Undo(IAbilityUser user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            UndoForTargets(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void UndoForTargets(IAbilityUser user, params GridObject[] targets) => UndoForTargets(user, targets.AsEnumerable());
        
        public void UndoForTargets(IAbilityUser user, IEnumerable<GridObject> targets)
        {
            UndoEffectsForTargets(user, targetEffects, targets);
            
            // It can be assumed that IAbilityUser can be converted to GridObject.
            if (user is GridObject userGridObject)
                UndoEffectsForTargets(user, userEffects, userGridObject);
        }

        private void UndoEffectsForTargets(IAbilityUser user, Effect[] effects, params GridObject[] targets) =>
            UndoEffectsForTargets(user, effects, targets.AsEnumerable());

        private void UndoEffectsForTargets(IAbilityUser user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            // TODO
        }

        /// <summary>
        /// Sum up all the values from each effect. Only count the effect if such effect can be used.
        /// </summary>
        private int CalculateValue(IAbilityUser user, IAbilityUser target, Effect[] effects, EffectValueType valueType) =>
            effects.Where(e => e.CanBeUsedBy(user, target))
                .Sum(e => e.CalculateValue(user, target, valueType));
    }
}
