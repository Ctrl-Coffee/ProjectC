using System;

public static class BattleUtility
{

    private static readonly Random Random = new Random();

    public static BattleUnitData CreatePlayerBattleUnitData(HeroInfoModel heroInfoModel, EquipmentData equipmentData)
    {
        BattleUnitData battleUnitData = new BattleUnitData();

        battleUnitData.MaxHp = heroInfoModel.Hp;
        battleUnitData.Attack = heroInfoModel.Attack;
        battleUnitData.Defense = heroInfoModel.Defense;
        battleUnitData.CriticalChance = heroInfoModel.CriticalChance;
        battleUnitData.BasicAttackHaste = heroInfoModel.BasicAttackHaste;
        battleUnitData.SignatureSkillHaste = heroInfoModel.SignatureSkillHaste;
        battleUnitData.BasicAttackSkillId = equipmentData.BasicAttackSkillId;
        battleUnitData.SignatureSkillId = equipmentData.SignatureSkillId;
        battleUnitData.CombatPower = heroInfoModel.CombatPower;

        battleUnitData.AnimationSetKey = equipmentData.AnimationSetKey;
        return battleUnitData;
    }

    public static BattleUnitData CreateCompanionBattleUnitData(CompanionState companionState, CompanionData companionData)
    {
        BattleUnitData battleUnitData = new BattleUnitData();

        battleUnitData.MaxHp = companionState.Hp;
        battleUnitData.Attack = companionState.Attack;
        battleUnitData.Defense = companionState.Defense;
        battleUnitData.CriticalChance = companionState.CriticalChance;
        battleUnitData.BasicAttackHaste = companionState.BasicAttackHaste;
        battleUnitData.SignatureSkillHaste = companionState.SignatureSkillHaste;
        battleUnitData.BasicAttackSkillId = companionData.BasicAttackSkillId;
        battleUnitData.SignatureSkillId = companionData.SignatureSkillId;
        battleUnitData.CombatPower = companionState.CombatPower;
        battleUnitData.AnimationSetKey = companionData.AnimationSetKey;

        return battleUnitData;
    }

    public static BattleUnitData CreateEnemyBattleUnitData(EnemyData enemyData, float multiplier)
    {
        BattleUnitData battleUnitData = new BattleUnitData();

        battleUnitData.MaxHp = enemyData.BaseHp * multiplier;
        battleUnitData.Attack = enemyData.BaseAttack * multiplier;
        battleUnitData.Defense = enemyData.BaseDefense * multiplier;
        battleUnitData.CriticalChance = enemyData.BaseCriticalChance;
        battleUnitData.BasicAttackHaste = enemyData.BasicAttackHaste;
        battleUnitData.SignatureSkillHaste = enemyData.SignatureSkillHaste;
        battleUnitData.BasicAttackSkillId = enemyData.BasicAttackSkillId;
        battleUnitData.SignatureSkillId = enemyData.SignatureSkillId;
        battleUnitData.CombatPower = CalculateCombatPower(battleUnitData);
        battleUnitData.AnimationSetKey = enemyData.AnimationSetKey;

        return battleUnitData;
    }

    public static string CreateUniqueId()
    {
        Guid newUid = Guid.NewGuid();

        string createdUid = newUid.ToString("N");

        return createdUid;
    }

    public static DamageResult CalculateDamage(AttackStats attackStats, DefenseStats defenseStats)
    {
        float baseDamage = attackStats.Damage;

        float reducedDamage = (baseDamage * Const.RATE_TO_PERCENT) / (Const.RATE_TO_PERCENT + defenseStats.Defense);

        bool isCritical = CalculateCritical(attackStats.CriticalChance);

        float calculatedDamage = isCritical ? reducedDamage * Const.CRITICAL_DAMAGE_MULTIPLIER : reducedDamage;

        DamageResult damageResult = new DamageResult(calculatedDamage, isCritical);
        return damageResult;
    }

    public static float CalculateCooldown(float baseCooldown, float skillHaste)
    {
        float calculatedCooldown = baseCooldown * 100 / (100 + skillHaste);
        return calculatedCooldown;
    } 

    private static bool CalculateCritical(float criticalChance)
    {
        bool isCritical = Random.NextDouble() < criticalChance;
        return isCritical;
    }

    private static float CalculateCombatPower(BattleUnitData battleUnitData)
    {
        float attackPower = battleUnitData.Attack * 10f * (1f + battleUnitData.CriticalChance * 0.5f);
        float hpPower = battleUnitData.MaxHp;
        float defensePower = battleUnitData.Defense * 10f;
        float basicAttackHastePower = battleUnitData.BasicAttackHaste * 5f;
        float signatureSkillHastePower = battleUnitData.SignatureSkillHaste * 5f;

        float calculatedCombatPower = attackPower + hpPower + defensePower + basicAttackHastePower + signatureSkillHastePower;

        return calculatedCombatPower;
    }
}
