using System;
using UnityEngine;

[Serializable]
public class WorkStatData : BaseData
{
    public string Name;
    public string RoundMode;
    public float MinValue;
    public float MaxValue;
    public float MinRate;
    public float MaxRate;
    public string Description;

    private WorkStatRoundMode _roundMode;
    private bool _isParsed = false;

    public WorkStatRoundMode RoundType
    {
        get
        {
            EnsureParsed();
            return _roundMode;
        }
    }

    public float Apply(float value, float rate)
    {
        if (0f < MinRate)
        {
            rate = Mathf.Max(rate, MinRate);
        }

        if (0f < MaxRate)
        {
            rate = Mathf.Min(rate, MaxRate);
        }

        float result = ApplyRound(value * rate);

        result = Mathf.Max(result, MinValue);

        if (0f < MaxValue)
        {
            result = Mathf.Min(result, MaxValue);
        }

        return result;
    }

    private float ApplyRound(float value)
    {
        switch (RoundType)
        {
            case WorkStatRoundMode.Floor:
                return Mathf.Floor(value);

            case WorkStatRoundMode.Ceil:
                return Mathf.Ceil(value);

            default:
                return Mathf.Round(value);
        }
    }

    private void EnsureParsed()
    {
        if (_isParsed)
        {
            return;
        }

        _isParsed = true;

        _roundMode = Utils.ParseEnum<WorkStatRoundMode>(RoundMode);
    }
}
