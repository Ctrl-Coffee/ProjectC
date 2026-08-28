using System;

[Serializable]
public class SkillData : BaseData
{
    public string Name;
    public string SkillType;
    public string TargetType;
    public float DamageMultiplier;
    public float BaseCooldown;
    public string Description;
}