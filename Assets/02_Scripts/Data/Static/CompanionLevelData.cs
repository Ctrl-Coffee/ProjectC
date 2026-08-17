using System;

[Serializable]
public class CompanionLevelData : BaseData
{
    public string CompanionId;
    public int Level;
    public float HP;
    public float BaseAttack;
    public float BaseDefense;
    public float UpgradeCost;
}
