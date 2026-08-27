using System;
using UnityEngine;

public static class BattleUtility
{
    private const int K = 100;

    public static string CreateUniqueId()
    {
        Guid newUid = Guid.NewGuid();

        string createdUid = newUid.ToString("N");

        return createdUid;
    }

    public static float CalculateDamage(SkillExecutionData attackStats, DefenseStats defenseStats)
    {
        float damage = 1;

        float defdamage = (damage * K) / (K + defenseStats.Defense);

        bool isCritical = CalculateCritical(attackStats.CriticalChance);

        return 500;
    }

    public static float CalculateBasicAttackSkillCooldown(float baseCooldown, float attackSpeed)
    {
        float calculatedCooldown = baseCooldown / (1f + attackSpeed);

        return calculatedCooldown;
    } 

    public static float CalculateSignatureSkillCooldown(float baseCooldown, float cooldownReduction)
    {
        cooldownReduction = Mathf.Clamp01(cooldownReduction);

        float calculatedCooldown = baseCooldown * (1f - cooldownReduction);

        return calculatedCooldown;
    }

    private static bool CalculateCritical(float critical)
    {
        bool isCritical = critical > 0;
        return isCritical;
    }
}
