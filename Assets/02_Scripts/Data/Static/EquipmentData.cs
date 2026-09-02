using System;

[Serializable]
public class EquipmentData : BaseData
{
    public string Name;
    public string Grade;    
    public float BaseAttack;
    public float BaseHp;
    public float BaseDefense;
    public float BaseCriticalChance;
    public float BasicAttackHaste;
    public float SignatureSkillHaste;
    public string EquipmentTypeString;
    public string BasicAttackSkillId;
    public string SignatureSkillId;
    public string IconSpriteAddressableKey;
    public string Description;
    public string AnimationSetKey;

    private EquipmentGrade _grade;
    private bool _isParsed = false;

    public EquipmentGrade EquipmentGrade
    {
        get
        {
            EnsureParsed();
            return _grade;
        }
    }

    private void EnsureParsed()
    {
        if (_isParsed) return;

        _isParsed = true;
        _grade = Utils.ParseEnum<EquipmentGrade>(Grade);
    }
}
