using System;

[Serializable]
public class PlayerLevelData : BaseData
{
    public int Level;
    public float BonusHP;
    public float BonusAttack;
    public float BonusDefense;
    public float BonusCriticalChance;
    public int UpgradeCost;
}
