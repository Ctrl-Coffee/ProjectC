using System;

[Serializable]
public class EquipmentData : BaseData
{
    public string Name;
    public float BaseAttack;
    public float BaseHp;
    public float BaseDefense;
    public string EquipmentTypeString;
    public string SkillId;
    public string IconPath;
    public string Grade;

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
