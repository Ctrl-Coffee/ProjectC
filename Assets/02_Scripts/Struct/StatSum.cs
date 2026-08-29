public struct HeroStatSum
{
    public float Attack;
    public float Hp;
    public float Defense;

    public float CriticalChance;
    public float BasicAttackHaste;
    public float BasicActiveSkillHaste;

    public void Add(HeroStatSum other)
    {
        Attack += other.Attack;
        Hp += other.Hp;
        Defense += other.Defense;
        CriticalChance += other.CriticalChance;
        BasicAttackHaste += other.BasicAttackHaste;
        BasicActiveSkillHaste += other.BasicActiveSkillHaste;
    }

    public bool IsSame(HeroStatSum other)
    {
        return Attack == other.Attack
            && Hp == other.Hp
            && Defense == other.Defense
            && CriticalChance == other.CriticalChance
            && BasicAttackHaste == other.BasicAttackHaste
            && BasicActiveSkillHaste == other.BasicActiveSkillHaste;
    }
}
