using System.Collections.Generic;
using UnityEngine;

public static class WorkStatTable
{
    private struct Rule
    {
        public WorkStatRoundMode RoundMode;
        public float MinValue;
        public float MaxValue;
        public float MinRate;
        public float MaxRate;
    }

    private static Dictionary<WorkStatType, Rule> _rules;

    public static float Apply(WorkStatType statType, float value, float rate)
    {
        EnsureBuilt();

        if (!_rules.TryGetValue(statType, out Rule rule))
        {
            Logger.LogWarning($"WorkStat 정의가 없어 상하한 없이 계산. stat: {statType}");
            return value * rate;
        }

        return ClampValue(rule, ApplyRound(rule, value * ClampRate(rule, rate)));
    }

    private static bool IsSet(float value)
    {
        return 0f <= value;
    }

    private static float ClampRate(Rule rule, float rate)
    {
        if (IsSet(rule.MinRate))
        {
            rate = Mathf.Max(rate, rule.MinRate);
        }

        if (IsSet(rule.MaxRate))
        {
            rate = Mathf.Min(rate, rule.MaxRate);
        }

        return rate;
    }

    private static float ClampValue(Rule rule, float value)
    {
        if (IsSet(rule.MinValue))
        {
            value = Mathf.Max(value, rule.MinValue);
        }

        if (IsSet(rule.MaxValue))
        {
            value = Mathf.Min(value, rule.MaxValue);
        }

        return value;
    }

    private static float ApplyRound(Rule rule, float value)
    {
        switch (rule.RoundMode)
        {
            case WorkStatRoundMode.Round:
                return Mathf.Round(value);

            case WorkStatRoundMode.Floor:
                return Mathf.Floor(value);

            case WorkStatRoundMode.Ceil:
                return Mathf.Ceil(value);

            default:
                return value;
        }
    }

    private static void EnsureBuilt()
    {
        if (null != _rules)
        {
            return;
        }

        _rules = new Dictionary<WorkStatType, Rule>();

        foreach (KeyValuePair<string, WorkStatData> pair in GameManager.DataTable.WorkStatDataTable)
        {
            WorkStatType statType = Utils.ParseEnum<WorkStatType>(pair.Key);

            if (WorkStatType.None == statType)
            {
                Logger.LogError($"WorkStat Id를 찾을 수 없음. id: {pair.Key}");
                continue;
            }

            if (_rules.ContainsKey(statType))
            {
                Logger.LogError($"WorkStat Id 가 중복. id: {pair.Key}");
                continue;
            }

            _rules.Add(statType, CreateRule(pair.Key, pair.Value));
        }
    }

    private static Rule CreateRule(string id, WorkStatData data)
    {
        Rule rule = new Rule();

        rule.RoundMode = string.IsNullOrEmpty(data.RoundMode)
            ? WorkStatRoundMode.None
            : Utils.ParseEnum<WorkStatRoundMode>(data.RoundMode);

        rule.MinValue = data.MinValue;
        rule.MinRate = data.MinRate;

        rule.MaxValue = ValidateUpperBound(id, nameof(data.MaxValue), data.MaxValue);
        rule.MaxRate = ValidateUpperBound(id, nameof(data.MaxRate), data.MaxRate);

        return rule;
    }

    private static float ValidateUpperBound(string id, string fieldName, float value)
    {
        if (0f != value)
        {
            return value;
        }

        Logger.LogError($"상한이 0. 빈 칸은 -1 로 채우기. id: {id}, field: {fieldName}");

        return -1f;
    }
}
