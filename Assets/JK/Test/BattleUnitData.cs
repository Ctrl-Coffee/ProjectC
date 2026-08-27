public struct BattleUnitData
{
    public string UId;
    public float MaxHp;
    public float Attack;
    public float Defense;
    public float CriticalChance;
    public float CriticalDamageMultiplier;
    public float AttackSpeed;
    public float CooldownReduction;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
    public string AnimationSetKey;

    public BattleUnitData(string uId, float maxHp, float attack, float defense, float criticalChance, float criticalDamageMultiplier, float attackSpeed, float cooldownReduction, string basicAttackSkillId, string signatureSkillId, string animationSetKey)
    {
        UId = uId;
        MaxHp = maxHp;
        Attack = attack;
        Defense = defense;
        CriticalChance = criticalChance;
        CriticalDamageMultiplier = criticalDamageMultiplier;
        AttackSpeed = attackSpeed;
        CooldownReduction = cooldownReduction;
        BasicAttackSkillId = basicAttackSkillId;
        SignatureSkillId = signatureSkillId;
        AnimationSetKey = animationSetKey;
    }
}