using System;

[Serializable]
public class EquipmentLevelData : BaseData
{
    public string Grade;
    public int Level;
    public float StatMultiplier;
    public int UpgradeCost;

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