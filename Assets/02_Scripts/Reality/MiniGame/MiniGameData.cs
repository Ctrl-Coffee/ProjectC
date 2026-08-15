using UnityEngine;

public enum MiniGameType
{
    None,
    SubtitleEdit,
    MotionTracking,
    ScratchLottery
}

public enum MiniGameGrade
{
    Miss,
    Bad,
    Normal,
    Good,
    Perfect,
}

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

    public static MiniGameResult Completed(float accuracy)
    {
        float clamped = Mathf.Clamp01(accuracy);

        return new MiniGameResult
        {
            IsCompleted = true,
            Accuracy = clamped,
            Grade = MiniGameGradeTable.GetGrade(clamped),
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
