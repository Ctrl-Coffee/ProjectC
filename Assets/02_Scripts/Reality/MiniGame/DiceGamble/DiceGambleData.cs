public struct DiceModifier
{
    public int RollCount;
    public int MinimumValue;
    public int ResultBonus; 
}
public struct DiceGambleResult
{
    public int TargetValue;
    public int[] RolledValues;
    public int FinalValue;
    public bool IsSuccess;
    public bool IsCriticalSuccess;
    public bool IsCriticalFail;
}

