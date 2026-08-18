using System;

[Serializable]
public class SkillData : BaseData
{
    public string Name;
    public SkillType SkillType;
    public int TargetCount;
    public float CoolDown;
    public float BaseEffect;
    public float BuffDuration;
}
