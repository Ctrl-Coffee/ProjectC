

public struct StatSum
{
    public float Attack;
    public float Hp;
    public float Defense;
    public float CriticalRate;
    public float NormalSkillHaste;
    public float SpecialSkillHaste;

    public void Add(StatSum other)
    {
        Attack += other.Attack;
        Hp += other.Hp;
        Defense += other.Defense;
        CriticalRate += other.CriticalRate;
        NormalSkillHaste += other.NormalSkillHaste;
        SpecialSkillHaste += other.SpecialSkillHaste;
    }
}
