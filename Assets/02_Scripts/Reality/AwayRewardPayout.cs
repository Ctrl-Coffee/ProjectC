using System.Collections.Generic;

public static class AwayRewardPayout
{
    private static Dictionary<CurrencyType, Reward> _rewards = new()
    {
        { CurrencyType.Energy, new Reward() },
        { CurrencyType.Money, new Reward() },
        { CurrencyType.DreamPoint, new Reward() },
    };

    private static bool _isHolding;

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

        PayAll();

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

        PayTo(currencyType, reward, (long)(reward.Total * progress));
    }

    public static void PayAll()
    {
        foreach (KeyValuePair<CurrencyType, Reward> pair in _rewards)
        {
            PayTo(pair.Key, pair.Value, pair.Value.Total);
        }
    }

    private static void SetTotal(CurrencyType currencyType, long total)
    {
        Reward reward = _rewards[currencyType];

        reward.Total = 0 < total ? total : 0;
        reward.Paid = 0;
    }

    private static void PayTo(CurrencyType currencyType, Reward reward, long value)
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

        Grant(currencyType, delta);
    }

    private static void Grant(CurrencyType currencyType, long amount)
    {
        if (null == GameManager.Session)
        {
            return;
        }

        CurrencyModel currency = GameManager.Session.Currency;

        switch (currencyType)
        {
            case CurrencyType.Energy:
                currency.AddEnergy(amount);
                break;
            case CurrencyType.Money:
                currency.AddMoney(amount);
                break;
            case CurrencyType.DreamPoint:
                currency.AddDreamPoint(amount);
                break;
        }
    }

    private class Reward
    {
        public long Total;
        public long Paid;
    }
}
