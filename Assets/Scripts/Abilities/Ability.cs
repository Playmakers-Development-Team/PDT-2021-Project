using System;
using System.Collections.Generic;
using System.Linq;
using Commands.Shapes;
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

        public void Use(IUnit user, Vector2Int originCoordinate, Vector2 targetVector)
        {
            Use(user, shape.GetTargets(originCoordinate, targetVector));
        }

        public void Use(IUnit user, params IUnit[] targets) => Use(user, targets.AsEnumerable());
        
        public void Use(IUnit user, IEnumerable<IUnit> targets)
        {
            Use(user, targetEffects, targets);
            Use(user, userEffects, user);
        }

        private void Use(IUnit user, Effect[] effects, params IUnit[] targets) =>
            Use(user, effects, targets.AsEnumerable());

        private void Use(IUnit user, Effect[] effects, IEnumerable<IUnit> targets)
        {
            int damage = CalculateAmount(user, effects, EffectValueType.Damage);
            int defence = CalculateAmount(user, effects, EffectValueType.Defence);
            int attack = CalculateAmount(user, effects, EffectValueType.Attack);
            ProcessTenet(user, effects);

            foreach (IUnit target in targets)
            {
                target.AddAttack(attack);
                target.AddDefence(defence);
                target.TakeDamage(Mathf.RoundToInt(user.DealDamageModifier.Modify(damage)));
                
                // TODO: Knockback??
            }
        }

        private int CalculateAmount(IUnit user, Effect[] effects, EffectValueType valueType)
        {
            int bonus = 0;
            
            foreach (Effect effect in effects)
            {
                if (effect.CanUse(user))
                {
                    bonus += effect.CalculateModifier(user, valueType);
                }
            }

            return bonus;
        }

        private void ProcessTenet(IUnit user, Effect[] effects)
        {
            foreach (Effect effect in effects)
            {
                if (effect.CanUse(user))
                {
                    effect.Expend(user);
                    effect.Provide(user);
                }
            }
        }
    }
}
