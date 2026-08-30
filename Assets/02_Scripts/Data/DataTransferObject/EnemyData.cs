using System;

[Serializable]
public class EnemyData : BaseData
{
    public string Name;
    public float BaseHp;
    public float BaseAttack;
    public float BaseDefense;
    public float BaseCriticalChance;
    public float BasicAttackHaste;
    public float SignatureSkillHaste;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
    public string AnimationSetKey;
}
