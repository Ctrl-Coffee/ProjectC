using DG.Tweening;
using System;
using System.Collections.Generic;

public static class AwayReportFlow
{
    private const float MIN_AWAY_SECONDS = 60f;

    private const float SHOW_SAFETY_SECONDS = 5f;

    private static bool _isRealLobby;
    private static bool _isPending;

    private static TimeSpan _awayDuration;
    private static Tween _showSafety;

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
        KillShowSafety();

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

        KillShowSafety();

        _showSafety = DOVirtual.DelayedCall(SHOW_SAFETY_SECONDS, OnShowFailed).SetUpdate(true);

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

    private static void OnShowFailed()
    {
        _showSafety = null;

        if (!_isPending)
        {
            return;
        }

        Logger.LogWarning("자리비움 리포트가 뜨지 않아 조용히 정산합니다.");

        _isPending = false;

        SettleSilently();
    }

    private static void KillShowSafety()
    {
        if (null == _showSafety)
        {
            return;
        }

        _showSafety.Kill();
        _showSafety = null;
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
