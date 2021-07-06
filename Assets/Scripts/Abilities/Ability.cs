using System;
using System.Collections.Generic;
using System.Linq;
using Abilities.Shapes;
using Cysharp.Threading.Tasks;
using GridObjects;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject
    {
        [SerializeField, TextArea(4, 8)] private string description;
        [SerializeField] private BasicShapeData shape;
        // [SerializeField] private int knockback;

        [SerializeField] private Effect[] targetEffects;
        [SerializeField] private Effect[] userEffects;

        public string Description => description;
        public IShape Shape => shape;

        public void Use(IUnit user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            UseForTargets(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void UseForTargets(IUnit user, params GridObject[] targets) => UseForTargets(user, targets.AsEnumerable());
        
        public void UseForTargets(IUnit user, IEnumerable<GridObject> targets)
        {
            UseEffectsForTargets(user, targetEffects, targets);
            
            // It can be assumed that IUnit can be converted to GridObject.
            if (user is GridObject userGridObject)
                UseEffectsForTargets(user, userEffects, userGridObject);
        }

        private void UseEffectsForTargets(IUnit user, Effect[] effects, params GridObject[] targets) =>
            UseEffectsForTargets(user, effects, targets.AsEnumerable());

        private void UseEffectsForTargets(IUnit user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            foreach (GridObject target in targets)
            {
                if (target is IUnit targetUnit)
                {
                    int attack = CalculateValue(user, targetUnit, effects, EffectValueType.Attack);
                    int defence = CalculateValue(user, targetUnit,effects, EffectValueType.Defence);
                    int damage = CalculateValue(user, targetUnit, effects, EffectValueType.Damage);
                
                    targetUnit.TakeAttack(attack);
                    targetUnit.TakeDefence(defence);
                    // Attack modifiers should only work when the effect actually intends to do damage
                    if (damage > 0)
                        targetUnit.TakeDamage(Mathf.RoundToInt(user.Attack.Modify(damage)));
                
                    // Check if knockback is supported first, because currently it sometimes doesn't
                    //if (targetUnit.Knockback != null)
                        //targetUnit.TakeKnockback(knockback);
                    
                    foreach (Effect effect in effects)
                    {
                        effect.ProcessTenet(user, targetUnit);
                    }
                }
            }
        }
        
        public void Undo(IUnit user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            UndoForTargets(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void UndoForTargets(IUnit user, params GridObject[] targets) => UndoForTargets(user, targets.AsEnumerable());
        
        public void UndoForTargets(IUnit user, IEnumerable<GridObject> targets)
        {
            UndoEffectsForTargets(user, targetEffects, targets);
            
            // It can be assumed that IUnit can be converted to GridObject.
            if (user is GridObject userGridObject)
                UndoEffectsForTargets(user, userEffects, userGridObject);
        }

        private void UndoEffectsForTargets(IUnit user, Effect[] effects, params GridObject[] targets) =>
            UndoEffectsForTargets(user, effects, targets.AsEnumerable());

        private void UndoEffectsForTargets(IUnit user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            // TODO
        }

        private int CalculateValue(IUnit user, IUnit target, Effect[] effects, EffectValueType valueType)
        {
            int bonus = 0;
            
            foreach (Effect effect in effects)
            {
                if (effect.CanBeUsedBy(user, target))
                {
                    bonus += effect.CalculateValue(user, target, valueType);
                }
            }

            return bonus;
        }
    }
}
