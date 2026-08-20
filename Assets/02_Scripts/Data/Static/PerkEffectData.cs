using System;

[Serializable]
public class PerkEffectData : BaseData
{
    public string EffectType;
    public string StatType;
    public string ModType;
    public string TargetId;
    public string Description;

    private PerkEffectType _effectType;
    private WorkStatType _statType;
    private PerkModType _modType;
    private bool _isParsed = false;

    public PerkEffectType Type
    {
        get
        {
            EnsureParsed();
            return _effectType;
        }
    }

    public WorkStatType Stat
    {
        get
        {
            EnsureParsed();
            return _statType;
        }
    }

    public PerkModType Mod
    {
        get
        {
            EnsureParsed();
            return _modType;
        }
    }

    private void EnsureParsed()
    {
        if (_isParsed)
        {
            return;
        }

        _isParsed = true;

        _effectType = Utils.ParseEnum<PerkEffectType>(EffectType);

        _statType = string.IsNullOrEmpty(StatType) ? WorkStatType.None : Utils.ParseEnum<WorkStatType>(StatType);
        _modType = string.IsNullOrEmpty(ModType) ? PerkModType.None : Utils.ParseEnum<PerkModType>(ModType);
    }
}
