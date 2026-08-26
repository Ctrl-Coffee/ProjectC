using System;
using System.Collections.Generic;

public static class AwayReportCollector
{
    private static bool _isCollecting = false;
    private static long _awayStartTicks = 0;

    private static long _money = 0;
    private static long _dreamPoint = 0;
    private static long _energy = 0;
    private static int _workTotal = 0;

    private static Dictionary<string, int> _workCounts = new();

    public static bool IsCollecting
    {
        get
        {
            return _isCollecting;
        }
    }

    public static void MarkAway(long nowTicks)
    {
        Clear();

        _awayStartTicks = nowTicks;
        _isCollecting = true;
    }

    public static AwayReport EndCollect(long nowTicks)
    {
        if (!_isCollecting)
        {
            return new AwayReport();
        }

        long awayTicks = nowTicks - _awayStartTicks;

        if (awayTicks < 0)
        {
            Logger.LogWarning($"자리비움 시간이 음수라 0으로 처리합니다. ticks: {awayTicks}");
            awayTicks = 0;
        }

        AwayReport report = new AwayReport
        {
            AwayDuration = TimeSpan.FromTicks(awayTicks),
            Money = _money,
            DreamPoint = _dreamPoint,
            Energy = _energy,
            CompletedWorkCounts = _workCounts,
            CompletedWorkTotal = _workTotal,
        };

        _isCollecting = false;

        Clear();

        return report;
    }

    public static void RecordWork(string workId, long money, long dreamPoint)
    {
        if (!_isCollecting)
        {
            return;
        }

        _money += money;
        _dreamPoint += dreamPoint;
        _workTotal++;

        if (string.IsNullOrEmpty(workId))
        {
            return;
        }

        _workCounts.TryGetValue(workId, out int count);
        _workCounts[workId] = count + 1;
    }

    public static void RecordEnergy(long amount)
    {
        if (!_isCollecting)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        _energy += amount;
    }

    private static void Clear()
    {
        _awayStartTicks = 0;

        _money = 0;
        _dreamPoint = 0;
        _energy = 0;
        _workTotal = 0;

        _workCounts = new Dictionary<string, int>();
    }
}
