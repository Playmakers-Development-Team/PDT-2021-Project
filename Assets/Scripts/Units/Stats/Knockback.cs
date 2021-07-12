namespace Units.Stats
{
    public class Knockback
    {
        public ModifierStat TakeKnockbackModifier { get; }

        public Knockback(ModifierStat takeKnockbackModifier)
        {
            TakeKnockbackModifier = takeKnockbackModifier;
        }

        public int TakeKnockback(int amount)
        {
            int knockbackTaken = (int) TakeKnockbackModifier.Modify(amount);
            return knockbackTaken;
        }
    }
}
