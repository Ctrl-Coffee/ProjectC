using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public static class EnergyRecovery
{
    private const float RECOVER_INTERVAL = 60f;
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

    private static void Recover()
    {
        UserData user = GameManager.User;
        CurrencyModel currency = user.Currency;

        long nowTicks = GameManager.Time.UtcNow.Ticks;

        if (user.LastEnergyRecoverTicks <= 0 || nowTicks < user.LastEnergyRecoverTicks)
        {
            user.LastEnergyRecoverTicks = nowTicks;
            return;
        }

        if (currency.MaxEnergy <= currency.Energy)
        {
            user.LastEnergyRecoverTicks = nowTicks;
            return;
        }

        long intervalTicks = (long)(RECOVER_INTERVAL * TimeSpan.TicksPerSecond);
        long recoverCount = (nowTicks - user.LastEnergyRecoverTicks) / intervalTicks;

        if (recoverCount <= 0)
        {
            return;
        }

        user.LastEnergyRecoverTicks += recoverCount * intervalTicks;

        currency.AddEnergy(recoverCount * RECOVER_AMOUNT);
        GameManager.Save.Save();

        Logger.Log($"에너지 회복 {recoverCount} - 현재 {currency.Energy} / {currency.MaxEnergy}");
    }
}
