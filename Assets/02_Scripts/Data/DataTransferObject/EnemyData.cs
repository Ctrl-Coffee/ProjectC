using System;

[Serializable]
public class EnemyData : BaseData
{
    public string Name;
    public float BaseHp;
    public float BaseAttack;
    public float BaseDefense;
    public float BaseCriticalChance;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
}
