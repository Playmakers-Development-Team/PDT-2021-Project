using System;
using Units;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    [CreateAssetMenu(menuName = "Ability", fileName = "New Ability", order = 250)]
    public class Ability : ScriptableObject
    {
        [SerializeField] private int damage;
        [SerializeField] private int defence;
        [SerializeField] private int knockback;
        
        [SerializeField] private Effect[] effects;
        

        public void Use(UnitData user, UnitData target)
        {
            int damageBonus = CalculateBonus(user, true);
            int defenceBonus = CalculateBonus(user, false);

            target.Damage(damage + damageBonus);
            target.Defend(defence + defenceBonus);
        }

        private int CalculateBonus(UnitData user, bool isDamage)
        {
            int bonus = 0;
            
            foreach (Effect effect in effects)
            {
                if (!effect.CanUse(user))
                    continue;

                bonus += isDamage ? effect.CalculateDamageModifier(user) : effect.CalculateDefenceModifier(user);
                effect.Expend(user);
            }

            return bonus;
        }
    }
}
