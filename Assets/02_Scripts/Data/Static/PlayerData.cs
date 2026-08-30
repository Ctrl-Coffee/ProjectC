using System;

[Serializable]
public class PlayerData : BaseData
{
    public string Name;
    public float BaseHp;
    public float BaseAttack;
    public float BaseDefense;
    public float BaseCriticalChance;
    public float BasicAttackHaste;
    public float SignatureSkillHaste;
    public string NormalSkillId;
}
