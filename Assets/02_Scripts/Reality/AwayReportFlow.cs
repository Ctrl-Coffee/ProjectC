using System;

public static class AwayReportFlow
{
    // 팝업 띄우기 최소 자리비움 시간
    private const float MIN_AWAY_SECONDS = 60f;

    public static void SetAppActive(bool isActive)
    {
        if (null == GameManager.Session)
        {
            return;
        }

        if (!isActive)
        {
            AwayReportCollector.MarkAway(GameManager.Time.UtcNow.Ticks);
            return;
        }

        ShowReportIfNeeded();
    }

    private static void ShowReportIfNeeded()
    {
        if (!AwayReportCollector.IsCollecting)
        {
            return;
        }

        AwayReport report = SettleAndCollect();

        if (report.AwayDuration.TotalSeconds < MIN_AWAY_SECONDS)
        {
            return;
        }

        if (!report.HasAnything)
        {
            return;
        }

        GameManager.UI.OpenAwayReportUI(report);
    }

    private static AwayReport SettleAndCollect()
    {
        AutoWorkQueue.CollectCompleted();
        EnergyRecovery.Recover();

        return AwayReportCollector.EndCollect(GameManager.Time.UtcNow.Ticks);
    }

#if UNITY_EDITOR
    public static void DebugSimulateAway(TimeSpan duration)
    {
        if (null == GameManager.Session)
        {
            Logger.LogWarning("로딩이 끝나기 전에는 자리비움을 시뮬레이션할 수 없습니다.");
            return;
        }

        AwayReportCollector.MarkAway(GameManager.Time.UtcNow.Ticks);

        GameManager.Time.AddDebugTime(duration);

        ShowReportIfNeeded();
    }
#endif
}
