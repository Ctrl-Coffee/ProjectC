using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DiceRoller
{
    public const int DICE_SIDES = 20;
    private const int TARGET_MIN = 1;
    private const int TARGET_MAX = 19;
    

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
        return Random.Range(TARGET_MIN, TARGET_MAX + 1);
    }

    
}
