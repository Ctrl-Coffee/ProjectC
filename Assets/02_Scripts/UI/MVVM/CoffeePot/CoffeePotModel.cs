using System;

public class CoffeePotModel : ModelBase
{
    private const long USE_INTERVAL_SECONDS = 600;
    private const long USE_INTERVAL_TICKS = USE_INTERVAL_SECONDS * TimeSpan.TicksPerSecond;
    private const long ENERGY_AMOUNT = 10;

    private CurrencyModel _currency;

    private long _usedAtTicks;

    public CoffeePotModel(CurrencyModel currency)
    {
        _currency = currency;
    }

    public void Restore(long usedAtTicks)
    {
        UsedAtTicks = usedAtTicks;
    }

    public long UsedAtTicks
    {
        get
        {
            return _usedAtTicks;
        }
        private set
        {
            if (_usedAtTicks == value)
            {
                return;
            }

            _usedAtTicks = value;
            OnPropertyChanged();
        }
    }

    public bool IsReady
    {
        get
        {
            return GetRemainTicks() <= 0;
        }
    }

    public float RemainSeconds
    {
        get
        {
            long remainTicks = GetRemainTicks();

            if (remainTicks <= 0)
            {
                return 0f;
            }

            return (float)remainTicks / TimeSpan.TicksPerSecond;
        }
    }

    // 0 이면 방금 사용한 직후, 1 이면 다시 사용할 수 있는 상태
    public float ChargeProgress
    {
        get
        {
            long remainTicks = GetRemainTicks();

            if (remainTicks <= 0)
            {
                return 1f;
            }

            return 1f - (float)remainTicks / USE_INTERVAL_TICKS;
        }
    }

    public long TryUse()
    {
        if (!IsReady)
        {
            Logger.Log($"커피포트 사용 불가 - {RemainSeconds}초 남음");
            return 0;
        }

        long amount = GetCappedAmount(ENERGY_AMOUNT);

        if (amount <= 0)
        {
            Logger.Log($"커피포트 사용 불가 - 에너지가 가득 참 {_currency.Energy} / {_currency.MaxEnergy}");
            return 0;
        }

        UsedAtTicks = GameManager.Time.UtcNow.Ticks;

        _currency.AddEnergy(amount);

        Logger.Log($"커피포트 에너지 회복 {amount} - 현재 {_currency.Energy} / {_currency.MaxEnergy}");

        SaveUtil.RequestSaveCurrency();

        return amount;
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(UsedAtTicks));
    }

    private long GetRemainTicks()
    {
        if (_usedAtTicks <= 0)
        {
            return 0;
        }

        return _usedAtTicks + USE_INTERVAL_TICKS - GameManager.Time.UtcNow.Ticks;
    }

    private long GetCappedAmount(long amount)
    {
        long capped = Math.Min(_currency.MaxEnergy, _currency.Energy + amount) - _currency.Energy;

        return 0 < capped ? capped : 0;
    }
}
