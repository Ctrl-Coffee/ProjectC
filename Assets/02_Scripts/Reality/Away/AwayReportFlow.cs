using System;
using System.Collections.Generic;

public static class AwayReportFlow
{
    private const float MIN_AWAY_SECONDS = 60f;

    private static bool _isRealLobby;
    private static bool _isPending;

    private static TimeSpan _awayDuration;

    public static void SetAppActive(bool isActive)
    {
        if (null == GameManager.Session)
        {
            return;
        }

        if (!isActive)
        {
            AwayTimer.Start(GameManager.Time.UtcNow.Ticks);
            return;
        }

        OnReturn();
    }

    public static void SetRealLobbyActive(bool isActive)
    {
        _isRealLobby = isActive;

        if (isActive)
        {
            TryShowPending();
        }
    }

    public static void OnReportOpened()
    {
        _isPending = false;
    }

    public static void OnReportClosed(IReadOnlyList<ICurrencyEffectSource> sources)
    {
        AwayRewardPayout.Consume();

        CurrencyFlyEffect effect = null;

        if (null != sources && 0 < sources.Count)
        {
            effect = CurrencyFlyEffect.GetOrCreate();
        }

        if (null == effect)
        {
            AwayRewardPayout.PayAll();
            return;
        }

        effect.Play(sources, AwayRewardPayout.PayProgress, AwayRewardPayout.PayAll);
    }

    // [주의] 정산 루프(AutoWorkQueue / EnergyRecovery)보다 반드시 먼저 불러야 한다.
    // 루프가 먼저 돌면 오프라인 보상이 조용히 지급되고 리포트가 0 으로 뜬다.
    public static void OnRelaunch()
    {
        if (null == GameManager.Session)
        {
            return;
        }

        long lastSeenTicks = GameManager.Session.Currency.EnergyRecoveredAt;

        if (lastSeenTicks <= 0)
        {
            return;
        }

        long awayTicks = GameManager.Time.UtcNow.Ticks - lastSeenTicks;

        if (awayTicks <= 0)
        {
            return;
        }

        TimeSpan duration = TimeSpan.FromTicks(awayTicks);

        if (duration.TotalSeconds < MIN_AWAY_SECONDS)
        {
            return;
        }

        _awayDuration = duration;
        _isPending = true;

        AwayRewardPayout.BeginHold();
    }

    private static void OnReturn()
    {
        if (!AwayTimer.IsRunning)
        {
            return;
        }

        _awayDuration = AwayTimer.Stop(GameManager.Time.UtcNow.Ticks);

        if (_awayDuration.TotalSeconds < MIN_AWAY_SECONDS)
        {
            SettleSilently();
            return;
        }

        _isPending = true;

        // [주의] 여기서 자동 정산을 멈춘다. 리포트를 닫을 때(OnReportClosed)만 풀린다.
        // 게임 중에 자동업무 보상이나 에너지 회복이 안 들어오면 이 보류가 안 풀린 것이다.
        // AwayRewardPayout.IsHolding 이 true 로 남아 있는지부터 확인할 것.
        AwayRewardPayout.BeginHold();

        TryShowPending();
    }

    private static void TryShowPending()
    {
        if (!_isPending || !_isRealLobby)
        {
            return;
        }

        AwayReport report = BuildPreview();

        if (!report.HasAnything)
        {
            _isPending = false;

            SettleSilently();
            return;
        }

        GameManager.UI.OpenAwayReportUI(report);
    }

    private static AwayReport BuildPreview()
    {
        AutoWorkQueue.Reward reward = AutoWorkQueue.PeekCompletedReward();

        return new AwayReport
        {
            AwayDuration = _awayDuration,
            Money = reward.Money,
            DreamPoint = reward.DreamPoint,
            Energy = EnergyRecovery.PeekRecoverAmount(),
            CompletedWorkCounts = reward.WorkCounts,
            CompletedWorkTotal = reward.Count,
        };
    }

    private static void SettleSilently()
    {
        AwayRewardPayout.ReleaseHold();

        AutoWorkQueue.CollectCompleted();
        EnergyRecovery.Recover();
    }

#if UNITY_EDITOR
    public static void DebugSimulateAway(TimeSpan duration)
    {
        if (null == GameManager.Session)
        {
            Logger.LogWarning("로딩이 끝나기 전에는 자리비움을 시뮬레이션할 수 없습니다.");
            return;
        }

        AwayTimer.Start(GameManager.Time.UtcNow.Ticks);

        GameManager.Time.AddDebugTime(duration);

        OnReturn();
    }
#endif
}
