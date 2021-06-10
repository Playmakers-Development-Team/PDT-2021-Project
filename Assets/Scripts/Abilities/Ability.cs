using System;
using System.Collections.Generic;
using System.Linq;
using Commands.Shapes;
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
        [SerializeField] private int knockback;
        [SerializeField] private bool onlyHitUnits = true;

        [SerializeField] private Effect[] targetEffects;
        [SerializeField] private Effect[] userEffects;

        public string Description => description;
        public IShape Shape => shape;

        public void Use(IUnit user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            Use(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void Use(IUnit user, params GridObject[] targets) => Use(user, targets.AsEnumerable());
        
        public void Use(IUnit user, IEnumerable<GridObject> targets)
        {
            Use(user, targetEffects, targets);
            
            // It can be assumed that IUnit can be converted to GridObject.
            if (user is GridObject userGridObject)
                Use(user, userEffects, userGridObject);
        }

        private void Use(IUnit user, Effect[] effects, params GridObject[] targets) =>
            Use(user, effects, targets.AsEnumerable());

        private void Use(IUnit user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            int damage = CalculateValue(user, effects, EffectValueType.Damage);
            int defence = CalculateValue(user, effects, EffectValueType.Defence);
            int attack = CalculateValue(user, effects, EffectValueType.Attack);
            ProcessTenets(user, effects);

            foreach (GridObject target in targets)
            {
                if (target is IUnit targetUnit)
                {
                    targetUnit.TakeAttack(attack);
                    targetUnit.TakeDefence(defence);
                    targetUnit.TakeDamage(Mathf.RoundToInt(user.DealDamageModifier.Modify(damage)));
                }

                target.TakeKnockback(knockback);
            }
        }
        
        public void Undo(IUnit user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            Undo(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void Undo(IUnit user, params GridObject[] targets) => Undo(user, targets.AsEnumerable());
        
        public void Undo(IUnit user, IEnumerable<GridObject> targets)
        {
            Undo(user, targetEffects, targets);
            
            // It can be assumed that IUnit can be converted to GridObject.
            if (user is GridObject userGridObject)
                Undo(user, userEffects, userGridObject);
        }

        private void Undo(IUnit user, Effect[] effects, params GridObject[] targets) =>
            Undo(user, effects, targets.AsEnumerable());

        private void Undo(IUnit user, Effect[] effects, IEnumerable<GridObject> targets)
        {
            int damage = CalculateValue(user, effects, EffectValueType.Damage);
            int defence = CalculateValue(user, effects, EffectValueType.Defence);
            int attack = CalculateValue(user, effects, EffectValueType.Attack);
            ProcessTenets(user, effects);

            foreach (GridObject target in targets)
            {
                if (target is IUnit targetUnit)
                {
                    targetUnit.TakeAttack(-attack);
                    targetUnit.TakeDefence(-defence);
                    targetUnit.TakeDamage(-Mathf.RoundToInt(user.DealDamageModifier.Modify(damage)));
                }
                
                target.TakeKnockback(-knockback);
            }
        }

        // TODO: Test
        private int CalculateValue(IUnit user, Effect[] effects, EffectValueType valueType)
        {
            int bonus = 0;
            
            foreach (Effect effect in effects)
            {
                if (effect.CanBeUsedBy(user))
                {
                    bonus += effect.CalculateValue(user, valueType);
                }
            }

            return bonus;
        }

        private void ProcessTenets(IUnit user, Effect[] effects)
        {
            foreach (Effect effect in effects)
            {
                effect.Use(user);
            }
        }
    }
}
