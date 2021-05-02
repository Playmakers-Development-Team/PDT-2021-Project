namespace GridObjects
{
    public class TakeWeightedKnockbackBehaviour : ITakeKnockbackBehaviour
    {
        private float weight;
        
        public TakeWeightedKnockbackBehaviour(float weight)
        {
            this.weight = weight;
        }

        public int TakeKnockback(int amount)
        {
            return (int) (amount * weight);
        }
    }
}
