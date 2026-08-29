using System.Collections.Generic;

public class PerkUnlockChecker
{
    private HashSet<string> _gatedIds;
    private HashSet<string> _unlockedIds;

    public void Invalidate()
    {
        _unlockedIds = null;
    }

    public bool IsUnlocked(string targetId)
    {
        if (string.IsNullOrEmpty(targetId))
        {
            return true;
        }

        EnsureGatedBuilt();

        if (!_gatedIds.Contains(targetId))
        {
            return true;
        }

        EnsureUnlockedBuilt();

        return _unlockedIds.Contains(targetId);
    }

    private void EnsureGatedBuilt()
    {
        if (null != _gatedIds)
        {
            return;
        }

        _gatedIds = new HashSet<string>();

        foreach (KeyValuePair<string, PerkEffectData> pair in GameManager.DataTable.PerkEffectDataTable)
        {
            PerkEffectData effect = pair.Value;

            if (PerkEffectType.Unlock != effect.Type)
            {
                continue;
            }

            if (string.IsNullOrEmpty(effect.TargetId))
            {
                Logger.LogError($"Unlock 효과인데 TargetId 가 비어 있습니다. effect: {pair.Key}");
                continue;
            }

            _gatedIds.Add(effect.TargetId);
        }
    }

    private void EnsureUnlockedBuilt()
    {
        if (null != _unlockedIds)
        {
            return;
        }

        _unlockedIds = new HashSet<string>();

        IReadOnlyList<string> perkIds = GameManager.Perk.GetUnlockedPerkIds();

        for (int i = 0; i < perkIds.Count; i++)
        {
            AccumulatePerk(perkIds[i]);
        }
    }

    private void AccumulatePerk(string perkId)
    {
        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data || null == data.EffectId)
        {
            return;
        }

        for (int i = 0; i < data.EffectId.Length; i++)
        {
            string effectId = data.EffectId[i];

            if (string.IsNullOrEmpty(effectId))
            {
                continue;
            }

            PerkEffectData effect = GameManager.DataTable.GetPerkEffectData(effectId);

            if (null == effect || PerkEffectType.Unlock != effect.Type)
            {
                continue;
            }

            if (string.IsNullOrEmpty(effect.TargetId))
            {
                continue;
            }

            _unlockedIds.Add(effect.TargetId);
        }
    }
}
