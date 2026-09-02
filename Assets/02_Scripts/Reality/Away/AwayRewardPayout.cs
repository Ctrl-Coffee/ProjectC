using System.Collections.Generic;

public static class AwayRewardPayout
{
    private static Dictionary<CurrencyType, Reward> _rewards = CreateRewards();

    private static bool _isHolding;

    // [주의] true 인 동안 AutoWorkQueue / EnergyRecovery 의 정산 루프가 통째로 멈춘다.
    // 자동업무 보상이나 에너지 회복이 안 들어온다는 제보가 오면 여기부터 볼 것.
    // BeginHold 는 AwayReportFlow.OnReturn 에서만 부르고, 리포트를 닫아야 풀린다.
    public static bool IsHolding
    {
        get
        {
            return _isHolding;
        }
    }

    public static void BeginHold()
    {
        _isHolding = true;
    }

    public static void ReleaseHold()
    {
        _isHolding = false;
    }

    public static void Consume()
    {
        ReleaseHold();

        FlushPending();

        AutoWorkQueue.Reward reward = AutoWorkQueue.ConsumeCompleted();

        SetTotal(CurrencyType.Energy, EnergyRecovery.ConsumeRecover());
        SetTotal(CurrencyType.Money, reward.Money);
        SetTotal(CurrencyType.DreamPoint, reward.DreamPoint);
    }

    public static void PayProgress(CurrencyType currencyType, float progress)
    {
        if (!_rewards.TryGetValue(currencyType, out Reward reward))
        {
            return;
        }

        PayTo(reward, (long)(reward.Total * progress));
    }

    public static void PayAll()
    {
        foreach (Reward reward in _rewards.Values)
        {
            PayTo(reward, reward.Total);
        }
    }

    private static Dictionary<CurrencyType, Reward> CreateRewards()
    {
        Dictionary<CurrencyType, Reward> rewards = new();

        rewards.Add(CurrencyType.Energy, new Reward(CurrencyType.Energy));
        rewards.Add(CurrencyType.Money, new Reward(CurrencyType.Money));
        rewards.Add(CurrencyType.DreamPoint, new Reward(CurrencyType.DreamPoint));

        return rewards;
    }

    // 이전 정산이 다 지급되지 않고 남아 있으면 먼저 털어낸다.
    private static void FlushPending()
    {
        PayAll();
    }

    private static void SetTotal(CurrencyType currencyType, long total)
    {
        Reward reward = _rewards[currencyType];

        reward.Total = 0 < total ? total : 0;
        reward.Paid = 0;
    }

    private static void PayTo(Reward reward, long value)
    {
        if (value > reward.Total)
        {
            value = reward.Total;
        }

        long delta = value - reward.Paid;

        if (delta <= 0)
        {
            return;
        }

        reward.Paid = value;

        Grant(reward.CurrencyType, delta);
    }

    private static void Grant(CurrencyType currencyType, long amount)
    {
        if (null == GameManager.Session)
        {
            return;
        }

        CurrencyModel currency = GameManager.Session.Currency;

        currency.Add(currencyType, amount);
    }

    private class Reward
    {
        public CurrencyType CurrencyType;

        public long Total;
        public long Paid;

        public Reward(CurrencyType currencyType)
        {
            CurrencyType = currencyType;
        }
    }
}
