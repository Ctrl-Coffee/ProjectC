using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoBattleDropTable", menuName = "Scriptable Objects/AutoBattleDropTable")]
public class AutoBattleDropTable : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public CurrencyType CurrencyType;
        public int Weight;
    }

    [SerializeField] private Entry[] _entries;

    public bool TryPick(out Entry picked)
    {
        picked = default;

        int totalWeight = GetTotalWeight();

        if (0 >= totalWeight)
        {
            return false;
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < _entries.Length; i++)
        {
            if (false == IsUsable(_entries[i]))
            {
                continue;
            }

            roll -= _entries[i].Weight;

            if (0 > roll)
            {
                picked = _entries[i];
                return true;
            }
        }

        return false;
    }

    private int GetTotalWeight()
    {
        if (null == _entries)
        {
            return 0;
        }

        int totalWeight = 0;

        for (int i = 0; i < _entries.Length; i++)
        {
            if (false == IsUsable(_entries[i]))
            {
                continue;
            }

            totalWeight += _entries[i].Weight;
        }

        return totalWeight;
    }

    private bool IsUsable(Entry entry)
    {
        return 0 < entry.Weight;
    }
}
