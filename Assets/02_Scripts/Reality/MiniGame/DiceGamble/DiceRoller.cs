using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DiceRoller
{
    public const int DICE_SIDES = 20;
    private const int TARGET_MIN = 1;
    private const int HIGH_BAND_MIN = 11;
    private const int TARGET_MAX = 19;

    private const int LOW_BAND_WEIGHT = 30;
    private const int HIGH_BAND_WEIGHT = 70;

    public MiniGameResult Roll(int targetValue, DiceModifier modifier)
    {
        int rollCount = Mathf.Max(1, modifier.RollCount);
        int[] rolledValues = new int[rollCount];

        for (int index = 0; index < rollCount; index++)
        {
            rolledValues[index] = Random.Range(1, DICE_SIDES + 1);
        }

        int pureValue = Mathf.Max(rolledValues);
        bool isCriticalSuccess = pureValue == DICE_SIDES;
        bool isCriticalFail = pureValue == 1;
        int finalValue = Mathf.Max(pureValue, modifier.MinimumValue) + modifier.ResultBonus;

        bool isSuccess;

        if (isCriticalSuccess)
        {
            isSuccess = true;
        }
        else if (isCriticalFail)
        {
            isSuccess = false;
        }
        else
        {
            isSuccess = finalValue >= targetValue;
        }

        return new MiniGameResult
        {
            TargetValue = targetValue,
            RolledValues = rolledValues,
            FinalValue = finalValue,
            IsSuccess = isSuccess,
            IsCriticalSuccess = isCriticalSuccess,
            IsCriticalFail = isCriticalFail,
        };
    }

    public int CreateTarget()
    {
        int totalWeight = LOW_BAND_WEIGHT + HIGH_BAND_WEIGHT;

        bool isLowBand = Random.Range(0, totalWeight) < LOW_BAND_WEIGHT;

        return isLowBand ? Random.Range(TARGET_MIN, HIGH_BAND_MIN) : Random.Range(HIGH_BAND_MIN, TARGET_MAX + 1);
    }
}
