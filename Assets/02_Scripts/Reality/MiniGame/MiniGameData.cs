using UnityEngine;

// TODO: 난이도(업무 레벨) 개념이 들어오면 여기에 필드를 추가하고 MiniGameFlowHandler에서 채우기
// MiniGameBase / PlayAsync 시그니처를 건드리지 않으려고 빈 구조체로 유지 중. 지우지 말 것.
public struct MiniGameContext
{
}

public struct MiniGameResult
{
    public bool IsCompleted;
    public float Accuracy;

    public MiniGameGrade Grade;

    public float RewardMultiplier;

    public bool SkipResultPopup;

    public bool IsSuccess;

    // 주사위 전용 
    public int TargetValue;
    public int[] RolledValues;
    public int FinalValue;
    public bool IsCriticalSuccess;
    public bool IsCriticalFail;

    // 복권 전용 
    public ScratchSymbol[] Symbols;
    public bool[] Revealed;
    public ScratchSymbol MatchedSymbol;
    public int MatchedCount;

    public static MiniGameResult Completed(float accuracy)
    {
        float clamped = Mathf.Clamp01(accuracy);

        return new MiniGameResult
        {
            IsCompleted = true,
            Accuracy = clamped,
            Grade = MiniGameGradeTable.GetGrade(clamped),
            RewardMultiplier = 1f,
        };
    }

    public static MiniGameResult Canceled
    {
        get
        {
            return new MiniGameResult
            {
                IsCompleted = false,
                Accuracy = 0f,
                Grade = MiniGameGrade.Miss,
            };
        }
    }
}

public static class MiniGameScore
{
    private const float DICE_SUCCESS_RATE = 1f;
    private const float DICE_FAIL_RATE = 0.5f;
    private const float DICE_CRITICAL_FAIL_RATE = 0f;
    private const float DICE_CRITICAL_SUCCESS_MULTIPLIER = 2f;

    private static readonly float[] SCRATCH_SYMBOL_RATES = { 0.1f, 0.2f, 0.5f, 1f };

    private const float SCRATCH_MATCH_4_MULTIPLIER = 1.5f;
    private const float SCRATCH_MATCH_5_MULTIPLIER = 2f;

    public static MiniGameResult FromDice(MiniGameResult result)
    {
        float rate = DICE_FAIL_RATE;

        if (result.IsSuccess)
        {
            rate = DICE_SUCCESS_RATE;
        }
        else if (result.IsCriticalFail)
        {
            rate = DICE_CRITICAL_FAIL_RATE;
        }

        result.IsCompleted = true;
        result.Accuracy = Mathf.Clamp01(rate);
        result.Grade = MiniGameGradeTable.GetGrade(result.Accuracy);
        result.RewardMultiplier = result.IsCriticalSuccess ? DICE_CRITICAL_SUCCESS_MULTIPLIER : 1f;

        result.SkipResultPopup = true;

        return result;
    }

    public static MiniGameResult FromScratch(MiniGameResult result)
    {
        result.IsCompleted = true;
        result.RewardMultiplier = 1f;

        if (result.IsSuccess == false)
        {
            result.Accuracy = 0f;
            result.Grade = MiniGameGrade.Miss;

            return result;
        }

        result.Accuracy = Mathf.Clamp01(GetScratchSymbolRate(result.MatchedSymbol));
        result.RewardMultiplier = GetScratchCountMultiplier(result.MatchedCount);
        result.Grade = GetScratchGrade(result.MatchedSymbol);

        return result;
    }

    private static float GetScratchSymbolRate(ScratchSymbol symbol)
    {
        int rateIndex = (int)symbol - 1;

        if (rateIndex < 0 || SCRATCH_SYMBOL_RATES.Length <= rateIndex)
        {
            Debug.LogError($"심볼 배율을 찾을 수 없습니다. symbol: {symbol}");
            return 0f;
        }

        return SCRATCH_SYMBOL_RATES[rateIndex];
    }

    private static float GetScratchCountMultiplier(int matchedCount)
    {
        if (5 <= matchedCount)
        {
            return SCRATCH_MATCH_5_MULTIPLIER;
        }

        if (4 == matchedCount)
        {
            return SCRATCH_MATCH_4_MULTIPLIER;
        }

        return 1f;
    }

    private static MiniGameGrade GetScratchGrade(ScratchSymbol symbol)
    {
        switch (symbol)
        {
            case ScratchSymbol.Money: return MiniGameGrade.Perfect;
            case ScratchSymbol.Bed: return MiniGameGrade.Good;
            case ScratchSymbol.Coffee: return MiniGameGrade.Normal;
            case ScratchSymbol.Computer: return MiniGameGrade.Bad;
            default: return MiniGameGrade.Miss;
        }
    }

    // 개수 기반 - 전체 N개 중 K개 성공
    // TODO: 미니게임에서 사용 예정. 아직 호출부가 없어도 지우지 말 것
    public static float FromCount(int successCount, int totalCount)
    {
        if (totalCount <= 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)successCount / totalCount);
    }

    // 거리 기반 - perfectRadius 안이면 만점, tolerance 밖이면 0점, 그 사이는 선형 감소
    public static float FromDistance(float distance, float perfectRadius, float tolerance)
    {
        if (tolerance <= 0f)
        {
            return 0f;
        }

        if (distance <= perfectRadius)
        {
            return 1f;
        }

        float falloffRange = tolerance - perfectRadius;

        if (falloffRange <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(1f - ((distance - perfectRadius) / falloffRange));
    }
}

public static class MiniGameGradeTable
{
    private const float PERFECT_THRESHOLD = 0.9f;
    private const float GOOD_THRESHOLD = 0.75f;
    private const float NORMAL_THRESHOLD = 0.5f;
    private const float BAD_THRESHOLD = 0.25f;

    public static MiniGameGrade GetGrade(float accuracy)
    {
        if (accuracy >= PERFECT_THRESHOLD) return MiniGameGrade.Perfect;
        if (accuracy >= GOOD_THRESHOLD) return MiniGameGrade.Good;
        if (accuracy >= NORMAL_THRESHOLD) return MiniGameGrade.Normal;
        if (accuracy >= BAD_THRESHOLD) return MiniGameGrade.Bad;

        return MiniGameGrade.Miss;
    }
}
