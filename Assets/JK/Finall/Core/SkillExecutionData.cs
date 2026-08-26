public struct SkillExecutionData
{
    public float Attack;
    public float CriticalChance;
    public float CriticalDamageMultiplier;

    public SkillExecutionData(float attack, float criticalChance, float criticalDamageMultiplier)
    {
        Attack = attack;
        CriticalChance = criticalChance;
        CriticalDamageMultiplier = criticalDamageMultiplier;
    }
}