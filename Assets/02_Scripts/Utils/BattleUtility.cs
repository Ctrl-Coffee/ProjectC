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
        battleUnitData.BasicAttackSkillId = "enemy_skill_ch1_melee_basic";
       //battleUnitData.BasicAttackSkillId = equipmentData.BasicAttackSkillId;
        battleUnitData.SignatureSkillId = "enemy_skill_ch1_melee_basic";
        //battleUnitData.SignatureSkillId = equipmentData.SignatureSkillId;
        battleUnitData.AnimationSetKey = "Assets/07_ScriptableObject/Hero/HeroBaseArmor.asset";
        //battleUnitData.AnimationSetKey = equipmentData.AnimationSetKey;

        return battleUnitData;
    }

    public static BattleUnitData CreateCompanionBattleUnitData()
    {
        BattleUnitData battleUnitData = new BattleUnitData();

        //battleUnitData.MaxHp = companionData.BaseHp;
        //battleUnitData.Attack = companionData.BaseAttack;
        //battleUnitData.Defense = companionData.BaseDefense;
        //battleUnitData.CriticalChance = companionData.BaseCriticalChance;
        //battleUnitData.AttackSpeed = companionData.BasicAttackHaste;
        //battleUnitData.CooldownReduction = companionData.SignatureSkillHaste;
        //battleUnitData.BasicAttackSkillId = companionData.BasicAttackSkillId;
        //battleUnitData.SignatureSkillId = companionData.SignatureSkillId;
        //battleUnitData.AnimationSetKey = "TestAnimKey";

        return battleUnitData;
    }

    public static BattleUnitData CreateEnemyBattleUnitData(EnemyData enemyData)
    {
        BattleUnitData battleUnitData = new BattleUnitData();

        battleUnitData.MaxHp = enemyData.BaseHp;
        battleUnitData.Attack = enemyData.BaseAttack;
        battleUnitData.Defense = enemyData.BaseDefense;
        battleUnitData.CriticalChance = enemyData.BaseCriticalChance;
        battleUnitData.BasicAttackHaste = enemyData.BasicAttackHaste;
        battleUnitData.SignatureSkillHaste = enemyData.SignatureSkillHaste;
        battleUnitData.BasicAttackSkillId = enemyData.BasicAttackSkillId;
        battleUnitData.SignatureSkillId = enemyData.SignatureSkillId;
        battleUnitData.AnimationSetKey = enemyData.AnimationSetKey;

        return battleUnitData;
    }

    public static string CreateUniqueId()
    {
        Guid newUid = Guid.NewGuid();

        string createdUid = newUid.ToString("N");

        return createdUid;
    }

    public static float CalculateDamage(AttackStats attackStats, DefenseStats defenseStats)
    {
        float baseDamage = attackStats.Damage;

        float reducedDamage = (baseDamage * Const.RATE_TO_PERCENT) / (Const.RATE_TO_PERCENT + defenseStats.Defense);

        bool isCritical = CalculateCritical(attackStats.CriticalChance);

        float calculatedDamage = isCritical ? reducedDamage * Const.CRITICAL_DAMAGE_MULTIPLIER : reducedDamage;
        return calculatedDamage;
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
}
