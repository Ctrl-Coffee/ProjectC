public struct BattleUnitData
{
    public string UnitId;
    public float MaxHp;
    public float Attack;
    public float Defense;
    public float CriticalChance;
    public float CriticalDamageMultiplier;
    public float AttackSpeed;
    public float CooldownReduction;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
    public string Key;

    public BattleUnitData(string unitId, float maxHp, float attack, float defense, float criticalChance, float criticalDamageMultiplier, float attackSpeed, float cooldownReduction, string basicAttackSkillId, string signature, string animKey)
    {
        UnitId = unitId;
        MaxHp = maxHp;
        Attack = attack;
        Defense = defense;
        CriticalChance = criticalChance;
        CriticalDamageMultiplier = criticalDamageMultiplier;
        AttackSpeed = attackSpeed;
        CooldownReduction = cooldownReduction;
        BasicAttackSkillId = basicAttackSkillId;
        SignatureSkillId = signature;
        Key = animKey;
    }
}