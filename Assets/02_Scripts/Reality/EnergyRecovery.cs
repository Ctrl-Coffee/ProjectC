using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public static class EnergyRecovery
{
    private const float BASE_RECOVER_INTERVAL = 300f;
    private const float BASE_RECOVER_SPEED = 1f;
    private const long RECOVER_AMOUNT = 1;
    private const float CHECK_INTERVAL = 1f;

    public static async UniTaskVoid RunRecoverLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Recover();

            await UniTask.Delay(TimeSpan.FromSeconds(CHECK_INTERVAL), ignoreTimeScale: true, cancellationToken: token);
        }
    }

    public static void Recover()
    {
        CurrencyModel currency = GameManager.Session.Currency;

        long nowTicks = GameManager.Time.UtcNow.Ticks;
        long lastRecoverTicks = currency.EnergyRecoveredAt;

        if (lastRecoverTicks <= 0 || nowTicks < lastRecoverTicks)
        {
            currency.EnergyRecoveredAt = nowTicks;
            return;
        }

        if (currency.MaxEnergy <= currency.Energy)
        {
            currency.EnergyRecoveredAt = nowTicks;
            return;
        }

        long intervalTicks = GetRecoverIntervalTicks();
        long recoverCount = (nowTicks - lastRecoverTicks) / intervalTicks;

        if (recoverCount <= 0)
        {
            return;
        }

        currency.EnergyRecoveredAt = lastRecoverTicks + recoverCount * intervalTicks;

        // 실제로 오른 양만 기록하기
        long beforeEnergy = currency.Energy;

        currency.AddEnergy(recoverCount * RECOVER_AMOUNT);

        AwayReportCollector.RecordEnergy(currency.Energy - beforeEnergy);

        Logger.Log($"에너지 회복 {recoverCount * RECOVER_AMOUNT} - 현재 {currency.Energy} / {currency.MaxEnergy}");
    }

    private static long GetRecoverIntervalTicks()
    {
        float speed = GameManager.Perk.Stat.GetFloat(WorkStatType.EnergyRecoverRate, BASE_RECOVER_SPEED);

        if (speed <= 0f)
        {
            Logger.LogError($"회복 속도 배율이 0 이하입니다. speed: {speed}");
            speed = BASE_RECOVER_SPEED;
        }

        long intervalTicks = (long)(BASE_RECOVER_INTERVAL / speed * TimeSpan.TicksPerSecond);

        return intervalTicks > 0 ? intervalTicks : 1;
    }

    public static float BaseRecoverSpeed
    {
        get
        {
            return BASE_RECOVER_SPEED;
        }
    }

    public static float BaseIntervalSeconds
    {
        get
        {
            return BASE_RECOVER_INTERVAL;
        }
    }

    public static float RecoverIntervalSeconds
    {
        get
        {
            return (float)GetRecoverIntervalTicks() / TimeSpan.TicksPerSecond;
        }
    }

    public static float RecoverSpeed
    {
        get
        {
            return GameManager.Perk.Stat.GetFloat(WorkStatType.EnergyRecoverRate, BASE_RECOVER_SPEED);
        }
    }

#if UNITY_EDITOR
    public static void DebugShiftLastRecoverTicks(long shiftTicks)
    {
        GameManager.Session.Currency.EnergyRecoveredAt += shiftTicks;
    }
#endif
}
