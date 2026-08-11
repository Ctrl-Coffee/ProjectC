public enum MiniGameGrade
{
    Miss,
    Bad,
    Normal,
    Good,
    Perfect,
}

public struct MiniGameContext
{
    public string WorkId;
    public int WorkLevel;
}

public struct MiniGameResult
{
    public bool IsCompleted;
    public float Accuracy;

    public MiniGameGrade Grade;

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
