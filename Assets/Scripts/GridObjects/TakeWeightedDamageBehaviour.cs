namespace GridObjects
{
    public class TakeWeightedDamageBehaviour : ITakeDamageBehaviour
    {
        private float weight;

        public TakeWeightedDamageBehaviour(float weight)
        {
            this.weight = weight;
        }

        public int TakeDamage(int amount)
        {
            return (int) (amount * weight);
        }
    }
}
