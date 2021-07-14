namespace Units.Stats
{
    //TODO: Refactor names without the affix "Stat".
    public interface IStat
    {
        
        public HealthStat HealthStat { get; }

        public Stat AttackStat { get; }
        
        public Stat DefenceStat { get; }
        
        public Stat MovementPoints { get; }
        
        public Stat SpeedStat { get; }
        
        public Stat KnockbackStat { get; }
        
        
        
        
    }
}
