using System.Collections.Generic;

public static class MiniGameRewardPayout
{
    private static Dictionary<CurrencyType, Reward> _rewards = CreateRewards();

    private static bool _isClaimed;

    public static void Begin(long money, long dp)
    {
        PayAll();   // 이전 판 잔액이 남아 있으면 먼저 털어낸다 (FlushPending 미러)

        SetTotal(CurrencyType.Money, money);
        SetTotal(CurrencyType.DreamPoint, dp);

        _isClaimed = true;
    }

    public static bool ConsumeClaim()
    {
        bool claimed = _isClaimed;
        _isClaimed = false;
        return claimed;
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

        rewards.Add(CurrencyType.Money, new Reward(CurrencyType.Money));
        rewards.Add(CurrencyType.DreamPoint, new Reward(CurrencyType.DreamPoint));

        return rewards;
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

        switch (currencyType)
        {
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
        public CurrencyType CurrencyType;

        public long Total;
        public long Paid;

        public Reward(CurrencyType currencyType)
        {
            CurrencyType = currencyType;
        }
    }
}