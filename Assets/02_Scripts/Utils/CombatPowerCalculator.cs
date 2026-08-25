/// <summary>
/// 전투력 = (공격력 × 10 × (1 + 치명타율 × 0.5)) + (체력 × 1) + (방어력 × 10) + (기본 스킬 가속 × 5) + (특수 스킬 가속 × 5)
/// </summary>
public static class CombatPowerCalculator
{
    public static float Calculate(IStatData stat)
    {
        if (null == stat)
        {
            Logger.LogError("전투력을 계산할 대상이 없습니다.");
            return 0f;
        }

        float criticalMultiplier = 1f + stat.CriticalRate * Const.CRITICAL_WEIGHT;

        return (stat.Attack * Const.ATTACK_WEIGHT * criticalMultiplier)
            + (stat.Hp * Const.HEALTH_WEIGHT)
            + (stat.Defense * Const.DEFENSE_WEIGHT)
            + (stat.NormalSkillHaste * Const.NORMAL_SKILL_HASTE_WEIGHT)
            + (stat.SpecialSkillHaste * Const.SPECIAL_SKILL_HASTE_WEIGHT);
    }
}
