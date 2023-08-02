namespace Zikkurat
{
    public enum Race : byte
    {
        Terran = 0,
        Zerg = 1,
        Protoss = 2
    }
    public enum Selected : byte
    {
        Building = 0,
        Unit = 1
    }
    public enum UnitStat : byte
    {
        HP,
        MoveSpeed,
        MaxVelocity,
        FastAttackDamage,
        FastAttackTime,
        HeavyAttackDamage,
        HeavyAttackTime,
        MissProbability,
        DoubleDamageProbability,
        FastHeavyProbability,
        AttackRange
    }
}