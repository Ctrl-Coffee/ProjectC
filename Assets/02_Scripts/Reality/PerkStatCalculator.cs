using System.Collections.Generic;

public class PerkStatCalculator
{
    private struct StatModifier
    {
        public float Flat;
        public float AdditiveRate;
        public float CompoundRate;
    }

    private Dictionary<WorkStatType, StatModifier> _modifiers;

    public void Invalidate()
    {
        _modifiers = null;
    }

    public float GetFloat(WorkStatType statType, float baseValue)
    {
        return Calculate(statType, baseValue);
    }

    public int GetInt(WorkStatType statType, float baseValue)
    {
        return (int)Calculate(statType, baseValue);
    }

    public long GetLong(WorkStatType statType, float baseValue)
    {
        return (long)Calculate(statType, baseValue);
    }

    private float Calculate(WorkStatType statType, float baseValue)
    {
        EnsureBuilt();

        float flat = 0f;
        float rate = 1f;

        if (_modifiers.TryGetValue(statType, out StatModifier modifier))
        {
            flat = modifier.Flat;
            rate = (1f + modifier.AdditiveRate) * modifier.CompoundRate;
        }

        return WorkStatTable.Apply(statType, baseValue + flat, rate);
    }

#if UNITY_EDITOR
    public bool TryGetModifier(WorkStatType statType, out float flat, out float additiveRate, out float compoundRate)
    {
        EnsureBuilt();

        flat = 0f;
        additiveRate = 0f;
        compoundRate = 1f;

        if (!_modifiers.TryGetValue(statType, out StatModifier modifier))
        {
            return false;
        }

        flat = modifier.Flat;
        additiveRate = modifier.AdditiveRate;
        compoundRate = modifier.CompoundRate;

        return true;
    }
#endif

    private void EnsureBuilt()
    {
        if (null != _modifiers)
        {
            return;
        }

        _modifiers = new Dictionary<WorkStatType, StatModifier>();

        IReadOnlyList<string> unlockedIds = GameManager.Perk.GetUnlockedPerkIds();

        for (int i = 0; i < unlockedIds.Count; i++)
        {
            AccumulatePerk(unlockedIds[i]);
        }
    }

    private void AccumulatePerk(string perkId)
    {
        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data || null == data.EffectId)
        {
            return;
        }

        if (null == data.EffectValue || data.EffectId.Length != data.EffectValue.Length)
        {
            Logger.LogError($"EffectId 와 EffectValue 개수가 맞지 않습니다. id: {perkId}");
            return;
        }

        for (int i = 0; i < data.EffectId.Length; i++)
        {
            AccumulateEffect(perkId, data.EffectId[i], data.EffectValue[i]);
        }
    }

    private void AccumulateEffect(string perkId, string effectId, float effectValue)
    {
        if (string.IsNullOrEmpty(effectId))
        {
            return;
        }

        PerkEffectData effect = GameManager.DataTable.GetPerkEffectData(effectId);

        if (null == effect)
        {
            Logger.LogError($"테이블에 없는 효과입니다. perk: {perkId}, effect: {effectId}");
            return;
        }

        if (PerkEffectType.Stat != effect.Type)
        {
            return;
        }

        if (WorkStatType.None == effect.Stat)
        {
            Logger.LogError($"Stat 효과인데 StatType 이 비어 있습니다. effect: {effectId}");
            return;
        }

        Accumulate(effect.Stat, effect.Mod, effectValue);
    }

    private void Accumulate(WorkStatType statType, PerkModType modType, float value)
    {
        if (!_modifiers.TryGetValue(statType, out StatModifier modifier))
        {
            modifier = new StatModifier();
            modifier.CompoundRate = 1f;
        }

        switch (modType)
        {
            case PerkModType.Flat:
                modifier.Flat += value;
                break;
            case PerkModType.Additive:
                modifier.AdditiveRate += value;
                break;
            case PerkModType.Compound:
                modifier.CompoundRate *= 1f + value;
                break;
            default:
                Logger.LogError($"지원하지 않는 연산 방식. stat: {statType}, mod: {modType}");
                return;
        }

        _modifiers[statType] = modifier;
    }
}
