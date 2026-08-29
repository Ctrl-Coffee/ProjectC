using System;

public static class AwayTimer
{
    private static bool _isRunning;
    private static long _startTicks;

    public static bool IsRunning
    {
        get
        {
            return _isRunning;
        }
    }

    public static void Start(long nowTicks)
    {
        _startTicks = nowTicks;
        _isRunning = true;
    }

    public static TimeSpan Stop(long nowTicks)
    {
        if (!_isRunning)
        {
            return TimeSpan.Zero;
        }

        _isRunning = false;

        long awayTicks = nowTicks - _startTicks;

        _startTicks = 0;

        if (awayTicks < 0)
        {
            Logger.LogWarning($"자리비움 시간이 음수라 0으로 처리합니다. ticks: {awayTicks}");
            return TimeSpan.Zero;
        }

        return TimeSpan.FromTicks(awayTicks);
    }
}
