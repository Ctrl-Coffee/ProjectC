using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

public class PlayerStatViewModel : ViewModelBase<PlayerStatModel>
{
    // 퍽 보정 조회용으로 매번 Enum.GetValues를 돌리지 않도록 캐싱
    private static readonly WorkStatType[] WORK_STAT_TYPES = (WorkStatType[])Enum.GetValues(typeof(WorkStatType));
    private static readonly PropertyChangedEventArgs PERK_BUFFS_CHANGED = new(nameof(PerkBuffs));

    public string Name { get; private set; }
    public int Level { get; private set; }

    public float Attack { get; private set; }
    public float Hp { get; private set; }
    public float Defense { get; private set; }
    public float CriticalRate { get; private set; }
    public float NormalSkillHaste { get; private set; }
    public float SpecialSkillHaste { get; private set; }
    public float NormalSkillCooldownReduceRate { get; private set; }
    public float SpecialSkillCooldownReduceRate { get; private set; }
    public float CombatPower { get; private set; }

    public IReadOnlyList<PerkBuffInfo> PerkBuffs { get { return _perkBuffs; } }

    private List<PerkBuffInfo> _perkBuffs = new();
    private StringBuilder _valueBuilder = new();

    public PlayerStatViewModel(PlayerStatModel model) : base(model)
    {
    }

    public void SubscribePerkChanged()
    {
        GameManager.Perk.OnPerkChanged += OnPerkChanged;
    }

    public void UnSubscribePerkChanged()
    {
        GameManager.Perk.OnPerkChanged -= OnPerkChanged;
    }

    public override void InitializeModel()
    {
        RefreshStats();
        RefreshPerkBuffs();

        base.InitializeModel();
    }

    public override void UnBind()
    {
        UnSubscribePerkChanged();
        base.UnBind();
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        RefreshStats();

        base.OnPropertyChanged(sender, e);
    }

    private void OnPerkChanged()
    {
        RefreshPerkBuffs();

        OnPropertyChanged(this, PERK_BUFFS_CHANGED);
    }

    private void RefreshStats()
    {
        PlayerData playerData = GameManager.DataTable.GetPlayerData(CharacterId.PLAYER_DATA);

        Name = null == playerData ? string.Empty : playerData.Name;
        Level = _model.Level;

        Attack = _model.Attack;
        Hp = _model.Hp;
        Defense = _model.Defense;
        CriticalRate = _model.CriticalRate;
        NormalSkillHaste = _model.NormalSkillHaste;
        SpecialSkillHaste = _model.SpecialSkillHaste;

        NormalSkillCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(NormalSkillHaste);
        SpecialSkillCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(SpecialSkillHaste);
        CombatPower = _model.CombatPower;
    }

    /// <summary>
    /// 활성화된 퍽이 만들어낸 보정치를 스탯별로 모은다.
    /// </summary>
    private void RefreshPerkBuffs()
    {
        _perkBuffs.Clear();

        for (int i = 0; i < WORK_STAT_TYPES.Length; i++)
        {
            WorkStatType statType = WORK_STAT_TYPES[i];

            if (WorkStatType.None == statType)
            {
                continue;
            }

            if (!GameManager.Perk.Stat.TryGetModifier(statType, out float flat, out float additiveRate, out float compoundRate))
            {
                continue;
            }

            string value = BuildValueText(flat, additiveRate, compoundRate);

            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            WorkStatData statData = GameManager.DataTable.GetWorkStatData(statType);
            string statName = null == statData ? statType.ToString() : statData.Name;
            string iconKey = null == statData ? string.Empty : statData.IconKey;

            _perkBuffs.Add(new PerkBuffInfo(statName, value, iconKey));
        }
    }

    private string BuildValueText(float flat, float additiveRate, float compoundRate)
    {
        _valueBuilder.Clear();

        if (0f != flat)
        {
            AppendSign(flat);
            _valueBuilder.Append(flat.ToString("0.##"));
        }

        if (0f != additiveRate)
        {
            AppendSeparator();
            AppendSign(additiveRate);
            _valueBuilder.Append((additiveRate * Const.RATE_TO_PERCENT).ToString("0.#"));
            _valueBuilder.Append('%');
        }

        if (Const.NO_COMPOUND_RATE != compoundRate)
        {
            AppendSeparator();
            _valueBuilder.Append('x');
            _valueBuilder.Append(compoundRate.ToString("0.##"));
        }

        return _valueBuilder.ToString();
    }

    private void AppendSeparator()
    {
        if (0 == _valueBuilder.Length)
        {
            return;
        }

        _valueBuilder.Append(' ');
    }

    private void AppendSign(float value)
    {
        if (0f < value)
        {
            _valueBuilder.Append('+');
        }
    }
}
