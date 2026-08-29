public interface IStatData
{
    float Attack { get; }
    float Hp { get; }
    float Defense { get; }

    float CriticalChance { get; }
    float BasicAttackHaste { get; }
    float SignatureSkillHaste { get; }

    float CombatPower { get; }
}
