using System;
using UnityEngine;

[Serializable]
public class CurrencyModel : ModelBase
{
    [SerializeField] private long _money;
    [SerializeField] private long _dp;

    public long Money
    {
        get
        {
            return _money;
        }
        private set
        {
            if (_money == value)
            {
                return;
            }

            _money = value;
            OnPropertyChanged();
        }
    }

    public long DP
    {
        get
        {
            return _dp;
        }
        private set
        {
            if (_dp == value)
            {
                return;
            }

            _dp = value;
            OnPropertyChanged();
        }
    }

    public override void InitializeOnce()
    {
    }

    public void AddMoney(long amount)
    {
        if (amount == 0)
        {
            return;
        }

        Money = Math.Max(0, _money + amount);
    }

    public void AddDP(long amount)
    {
        if (amount == 0)
        {
            return;
        }

        DP = Math.Max(0, _dp + amount);
    }
}
