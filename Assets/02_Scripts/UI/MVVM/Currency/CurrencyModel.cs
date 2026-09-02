using System;
using UnityEngine;

[Serializable]
public class CurrencyModel : ModelBase
{
    // 테스트용 설정. 최대치 200, 시작치 50
    private const long START_ENERGY = 50;
    private const long MAX_ENERGY = 200;

    [SerializeField] private long _money;
    [SerializeField] private long _dreamPoint;
    [SerializeField] private long _energy = START_ENERGY;
    [SerializeField] private long _dreamFragment;
    [SerializeField] private long _dreamScroll;
    [SerializeField] private long _inspiration;
    [SerializeField] private long _energyRecoveredAt;

    public CurrencyModel(CurrencyDto currencyDto)
    {
        _money = currencyDto.money;
        _dreamPoint = currencyDto.dreamPoint;
        _energy = currencyDto.energy;
        _dreamFragment = currencyDto.dreamFragment;
        _dreamScroll = currencyDto.dreamScroll;
        _inspiration = currencyDto.inspiration;
        _energyRecoveredAt = currencyDto.energyRecoveredAt;
    }

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

    public long DreamPoint
    {
        get
        {
            return _dreamPoint;
        }
        private set
        {
            if (_dreamPoint == value)
            {
                return;
            }

            _dreamPoint = value;
            OnPropertyChanged();
        }
    }

    public long Energy
    {
        get
        {
            return _energy;
        }
        private set
        {
            if (_energy == value)
            {
                return;
            }

            _energy = value;
            OnPropertyChanged();
        }
    }

    public long MaxEnergy
    {
        get
        {
            return GameManager.Perk.Stat.GetLong(WorkStatType.EnergyMax, MAX_ENERGY);
        }
    }

    public void NotifyMaxEnergyChanged()
    {
        OnPropertyChanged(nameof(MaxEnergy));
    }

    public long EnergyRecoveredAt
    {
        get
        {
            return _energyRecoveredAt;
        }
        set
        {
            _energyRecoveredAt = value;
        }
    }

    public long BaseMaxEnergy
    {
        get
        {
            return MAX_ENERGY;
        }
    }

    public long DreamFragment
    {
        get
        {
            return _dreamFragment;
        }
        private set
        {
            if (_dreamFragment == value)
            {
                return;
            }

            _dreamFragment = value;
            OnPropertyChanged();
        }
    }

    public long DreamScroll
    {
        get
        {
            return _dreamScroll;
        }
        private set
        {
            if (_dreamScroll == value)
            {
                return;
            }

            _dreamScroll = value;
            OnPropertyChanged();
        }
    }

    public long Inspiration
    {
        get
        {
            return _inspiration;
        }
        private set
        {
            if (_inspiration == value)
            {
                return;
            }

            _inspiration = value;
            OnPropertyChanged();
        }
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Money));
        OnPropertyChanged(nameof(DreamPoint));
        OnPropertyChanged(nameof(Energy));
        OnPropertyChanged(nameof(DreamFragment));
        OnPropertyChanged(nameof(DreamScroll));
        OnPropertyChanged(nameof(Inspiration));
    }

    public void AddMoney(long amount)
    {
        Money += amount;
        SaveUtil.RequestSaveCurrency();
    }

    public void AddDreamPoint(long amount)
    {
        DreamPoint += amount;
        SaveUtil.RequestSaveCurrency();
    }

    public void AddEnergy(long amount)
    {
        long maxEnergy = MaxEnergy;

        if (maxEnergy <= Energy)
        {
            return;
        }

        Energy = Math.Min(maxEnergy, Energy + amount);
        SaveUtil.RequestSaveCurrency();
    }

    public void AddDreamFragment(long amount)
    {
        DreamFragment += amount;
        SaveUtil.RequestSaveCurrency();
    }
    
    public void AddDreamScroll(long amount)
    {
        DreamScroll += amount;
        SaveUtil.RequestSaveCurrency();
    }

    public void AddInspiration(long amount)
    {
        Inspiration += amount;
        SaveUtil.RequestSaveCurrency();
    }

    public void Add(CurrencyType currencyType, long amount)
    {
        if (amount <= 0)
        {
            return;
        }

        switch (currencyType)
        {
            case CurrencyType.Money:
                AddMoney(amount);
                break;
            case CurrencyType.DreamPoint:
                AddDreamPoint(amount);
                break;
            case CurrencyType.Energy:
                AddEnergy(amount);
                break;
            case CurrencyType.DreamFragment:
                AddDreamFragment(amount);
                break;
            case CurrencyType.DreamScroll:
                AddDreamScroll(amount);
                break;
            case CurrencyType.Inspiration:
                AddInspiration(amount);
                break;
            default:
                Logger.LogError($"지급할 수 없는 재화 종류입니다. {currencyType}");
                break;
        }
    }
    public bool CanSpendMoney(long amount)
    {
        return CanSpend(Money, amount);
    }

    public bool TrySpendMoney(long amount)
    {
        if (!CanSpend(Money, amount))
        {
            return false;
        }

        Money -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    public bool TrySpendDreamPoint(long amount)
    {
        if (!CanSpend(DreamPoint, amount))
        {
            return false;
        }

        DreamPoint -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    public bool CanSpendEnergy(long amount)
    {
        return CanSpend(Energy, amount);
    }

    public bool TrySpendEnergy(long amount)
    {
        if (!CanSpend(Energy, amount))
        {
            return false;
        }

        Energy -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    public bool TrySpendDreamFragment(long amount)
    {
        if (!CanSpend(DreamFragment, amount))
        {
            return false;
        }

        DreamFragment -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    public bool TrySpendDreamScroll(long amount)
    {
        if (!CanSpend(DreamScroll, amount))
        {
            return false;
        }

        DreamScroll -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    public bool TrySpendInspiration(long amount)
    {
        if (!CanSpend(Inspiration, amount))
        {
            return false;
        }

        Inspiration -= amount;
        SaveUtil.RequestSaveCurrency();

        return true;
    }

    private bool CanSpend(long current, long amount)
    {
        return 0 < amount && amount <= current;
    }
}
