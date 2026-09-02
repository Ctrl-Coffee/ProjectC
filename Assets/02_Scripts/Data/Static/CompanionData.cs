using System;

[Serializable]
public class CompanionData : BaseData
{
    public string Name;
    public int Grade;
    public float BaseHp;
    public float HpGrowthPerLevel;
    public float BaseAttack;
    public float AttackGrowthPerLevel;
    public float BaseDefense;
    public float DefenseGrowthPerLevel;
    public float BaseCriticalChance;
    public float BasicAttackHaste;
    public float SignatureSkillHaste;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
    public string SlotSpriteAddressableKey;
    public string AnimationSetKey;
}
