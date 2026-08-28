using UnityEngine;

/// <summary>
/// 최종 쿨타임 = 기본 쿨타임 × 100 ÷ (100 + 스킬 가속)
/// </summary>
public static class SkillCooldownCalculator
{
    public static float GetNormalSkillCooldown(float baseCooldown, IStatData stat)
    {
        return Calculate(baseCooldown, null == stat ? 0f : stat.NormalSkillHaste);
    }

    public static float GetSpecialSkillCooldown(float baseCooldown, IStatData stat)
    {
        return Calculate(baseCooldown, null == stat ? 0f : stat.SpecialSkillHaste);
    }

    public static float GetCooldownReduceRate(float haste)
    {
        return 1f - Calculate(1f, haste);
    }

    public static float Calculate(float baseCooldown, float haste)
    {
        float divisor = Const.HASTE_BASE + haste;

        float minDivisor = Const.HASTE_BASE * Const.MIN_COOLDOWN_RATE;

        divisor = Mathf.Max(divisor, minDivisor);

        return baseCooldown * Const.HASTE_BASE / divisor;
    }
}
