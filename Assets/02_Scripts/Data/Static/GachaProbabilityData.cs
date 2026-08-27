using System;

[Serializable]
public class GachaProbabilityData : BaseData
{
    public GachaType GachaType;
    public int Grade;
    public int Probability;
    public int DuplicateReward;
}
