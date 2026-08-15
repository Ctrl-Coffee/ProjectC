using System;

[Serializable]
public class PerkNodeData : BaseData
{
    private static readonly string[] EMPTY_IDS = new string[0];

    public string Name;
    public string AxisType;
    public string NodeType;
    public long InspirationCost;
    public string ParentMode;
    public string ParentId;
    public string EffectId;
    public string EffectValue;
    public string IconKey;
    public string Description;
    public string ExclusiveGroup;

    private string[] _parentIds;
    private string[] _effectIds;
    private string[] _effectValues;
    private bool _isParsed = false;

    public string[] ParentIds
    {
        get
        {
            EnsureParsed();
            return _parentIds;
        }
    }

    public string[] EffectIds
    {
        get
        {
            EnsureParsed();
            return _effectIds;
        }
    }

    public string[] EffectValues
    {
        get
        {
            EnsureParsed();
            return _effectValues;
        }
    }

    private void EnsureParsed()
    {
        if (_isParsed)
        {
            return;
        }

        _isParsed = true;

        _parentIds = Split(ParentId);
        _effectIds = Split(EffectId);
        _effectValues = Split(EffectValue);
    }

    private static string[] Split(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return EMPTY_IDS;
        }

        string[] split = source.Split(',');

        for (int i = 0; i < split.Length; i++)
        {
            split[i] = split[i].Trim();
        }

        return split;
    }
}
