namespace Units
{
    public abstract class UnitData
    {
        protected static int HealthPoints { get; set; }
        public int MovementActionPoints { get; set; }
        public float DamageModifier { get; set; }
        public float DefenceModifier { get; set; }
    }
}
