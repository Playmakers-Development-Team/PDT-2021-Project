using System;
using System.Collections.Generic;
using System.Linq;
using Commands.Shapes;
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
        [SerializeField] private int knockback;
        [SerializeField] private bool onlyHitUnits = true;

        [SerializeField] private Effect[] targetEffects;
        [SerializeField] private Effect[] userEffects;

        public string Description => description;
        public IShape Shape => shape;

        public void Use(IUnit user, Vector2Int originCoordinate, Vector2 targetVector, bool undo = false) =>
            Use(user, shape.GetTargets(originCoordinate, targetVector), undo);
        
        public void Use(IUnit user, IEnumerable<GridObject> targets, bool undo = false)
        {
            Use(user, targetEffects, targets, undo);
            
            // It can be assumed that IUnit can be converted to GridObject.
            if (user is GridObject userGridObject)
                Use(user, userEffects, new List<GridObject> {userGridObject}, undo);
        }

        private void Use(IUnit user, Effect[] effects, IEnumerable<GridObject> targets, bool undo = false)
        {
            // TODO: Use the undo parameter to invert values
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
                
                    // Check if knockback is supported first, because currently it sometimes isn't
                    if (targetUnit.Knockback != null)
                        targetUnit.TakeKnockback(knockback);
                    
                    foreach (Effect effect in effects)
                    {
                        effect.ProcessTenet(user, targetUnit);
                    }
                }
            }
        }
        
        // TODO: Test undo functions once undo has been implemented
        public void Undo(IUnit user, Vector2Int originCoordinate, Vector2 targetVector) =>
            Use(user, originCoordinate, targetVector, true);

        public void Undo(IUnit user, IEnumerable<GridObject> targets) =>
            Use(user, targets, true);

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
