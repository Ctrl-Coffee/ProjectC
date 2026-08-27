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
    public float BasicActiveSkillHaste;
    public string EquipmentTypeString;
    public string BaseSkillId;
    public string ActiveSkillId;
    public string IconPath;
    public string Description;

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
