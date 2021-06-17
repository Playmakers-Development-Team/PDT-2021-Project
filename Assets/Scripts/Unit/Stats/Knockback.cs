using Unit.Stats;
using UnityEngine;

namespace Unit
{
    public class Knockback
    {
        public ModifierStat TakeKnockbackModifier { get; }

        public Knockback(ModifierStat takeKnockbackModifier)
        {
            TakeKnockbackModifier = takeKnockbackModifier;
        }

        public void TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            Debug.Log(knockbackTaken + " knockback taken.");
        }
    }
}
