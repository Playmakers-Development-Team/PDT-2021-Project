using System;

namespace Units.Stats
{
    
    [Serializable][Obsolete("Knockback is not being used, use the Stat Class instead")]
    public class Knockback
    {
        public ModifierStat TakeKnockbackModifier { get; }

        public Knockback(ModifierStat takeKnockbackModifier) => TakeKnockbackModifier = takeKnockbackModifier;
        

        public int TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            return knockbackTaken;
        }
    }
}
