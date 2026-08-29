public interface IStatData
{
    float Attack { get; }
    float Hp { get; }
    float Defense { get; }

    float CriticalChance { get; }
    float BasicAttackHaste { get; }
    float BasicActiveSkillHaste { get; }

    float CombatPower { get; }
}
