using System.Collections.Generic;

public class AutoBattlePendingReward
{
    private readonly Dictionary<CurrencyType, long> _amounts = new Dictionary<CurrencyType, long>();

    public bool HasAny
    {
        get
        {
            foreach (long amount in _amounts.Values)
            {
                if (0 < amount)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void Add(CurrencyType currencyType, long amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _amounts.TryGetValue(currencyType, out long current);

        _amounts[currencyType] = current + amount;
    }

    public long GetAmount(CurrencyType currencyType)
    {
        _amounts.TryGetValue(currencyType, out long amount);

        return amount;
    }

    public void MoveTo(AutoBattlePendingReward target)
    {
        if (null == target)
        {
            return;
        }

        foreach (KeyValuePair<CurrencyType, long> pair in _amounts)
        {
            target.Add(pair.Key, pair.Value);
        }

        Clear();
    }

    public void Payout()
    {
        if (null == GameManager.Session)
        {
            return;
        }

        CurrencyModel currency = GameManager.Session.Currency;

        foreach (KeyValuePair<CurrencyType, long> pair in _amounts)
        {
            currency.Add(pair.Key, pair.Value);
        }

        Clear();
    }

    public void Clear()
    {
        _amounts.Clear();
    }
}
