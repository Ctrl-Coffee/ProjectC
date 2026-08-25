public interface IStatData
{
    float Attack { get; }
    float Hp { get; }
    float Defense { get; }

    float CriticalRate { get; }
    float NormalSkillHaste { get; }
    float SpecialSkillHaste { get; }

    float CombatPower { get; }
}
