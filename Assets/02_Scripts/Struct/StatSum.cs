

public struct StatSum
{
    public float Attack;
    public float Hp;
    public float Defense;
    public float CriticalChance;
    public float BasicAttackHaste;
    public float SignatureSkillHaste;

    public void Add(StatSum other)
    {
        Attack += other.Attack;
        Hp += other.Hp;
        Defense += other.Defense;
        CriticalChance += other.CriticalChance;
        BasicAttackHaste += other.BasicAttackHaste;
        SignatureSkillHaste += other.SignatureSkillHaste;
    }

    public bool IsSame(StatSum other)
    {
        return Attack == other.Attack
            && Hp == other.Hp
            && Defense == other.Defense
            && CriticalChance == other.CriticalChance
            && BasicAttackHaste == other.BasicAttackHaste
            && SignatureSkillHaste == other.SignatureSkillHaste;
    }
}
