using System;

[Serializable]
public class PlayerData : BaseData
{
    public string Name;
    public float BaseHp;
    public float BaseAttack;
    public float BaseDefense;
    public float BaseCritRate;
    public float BaseNormalSkillHaste;
    public float BaseSpecialSkillHaste;
    public string NormalSkillId;
}
