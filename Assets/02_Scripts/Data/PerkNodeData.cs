using System;

[Serializable]
public class PerkNodeData : BaseData
{
    public string Name;
    public string AxisType;
    public string NodeType;
    public int InspirationCost;
    public string ParentMode;
    public string[] ParentId;
    public string[] EffectId;
    public string[] EffectValue;
    public string IconKey;
    public string Description;
    public string ExclusiveGroup;
}
