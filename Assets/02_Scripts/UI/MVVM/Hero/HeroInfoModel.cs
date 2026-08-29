using System.ComponentModel;

public class HeroInfoModel : ModelBase, IStatData
{
    private OwnedPlayerData _ownedPlayerData;
    private HeroEquipedModel _equiped;
    private HeroEquipmentModel _equipment;

    private StatSum _baseStat;
    private StatSum _equipmentStat;

    private float _attack;
    private float _hp;
    private float _defense;
    private float _criticalChance;
    private float _basicAttackHaste;
    private float _basicActiveSkillHaste;
    private float _combatPower;
    private int _level;

    public int Level
    {
        get { return _level; }
        private set
        {
            if (_level == value) return;
            _level = value;
            OnPropertyChanged();
        }
    }

    public float Attack
    {
        get { return _attack; }
        private set
        {
            if (_attack == value) return;
            _attack = value;
            OnPropertyChanged();
        }
    }

    public float Hp
    {
        get { return _hp; }
        private set
        {
            if (_hp == value) return;
            _hp = value;
            OnPropertyChanged();
        }
    }

    public float Defense
    {
        get { return _defense; }
        private set
        {
            if (_defense == value) return;
            _defense = value;
            OnPropertyChanged();
        }
    }

    public float CriticalChance
    {
        get { return _criticalChance; }
        private set
        {
            if (_criticalChance == value) return;
            _criticalChance = value;
            OnPropertyChanged();
        }
    }

    public float BasicAttackHaste
    {
        get { return _basicAttackHaste; }
        private set
        {
            if (_basicAttackHaste == value) return;
            _basicAttackHaste = value;
            OnPropertyChanged();
        }
    }

    public float BasicActiveSkillHaste
    {
        get { return _basicActiveSkillHaste; }
        private set
        {
            if (_basicActiveSkillHaste == value) return;
            _basicActiveSkillHaste = value;
            OnPropertyChanged();
        }
    }

    public float CombatPower
    {
        get { return _combatPower; }
        private set
        {
            if (_combatPower == value) return;
            _combatPower = value;
            OnPropertyChanged();
        }
    }

    public StatSum BaseStat
    {
        get { return _baseStat; }
        private set
        {
            if (_baseStat.IsSame(value)) return;
            _baseStat = value;
            OnPropertyChanged();
        }
    }

    public StatSum EquipmentStat
    {
        get { return _equipmentStat; }
        private set
        {
            if (_equipmentStat.IsSame(value)) return;
            _equipmentStat = value;
            OnPropertyChanged();
        }
    }

    public HeroInfoModel(OwnedPlayerData ownedPlayerData, HeroEquipedModel equiped, HeroEquipmentModel equipment)
    {
        _ownedPlayerData = ownedPlayerData;
        _equiped = equiped;
        _equipment = equipment;

        // 장비 착용 / 장비 강화 시 최종 스텟이 바뀌므로 구독. 레벨은 이 모델이 직접 소유한다.
        _equiped.PropertyChanged += OnEquipedChanged;
        _equipment.ContainerPropertyChanged += OnEquipmentChanged;

        Recalculate();
    }

    public override void InitializeOnce()
    {
        Recalculate();
    }

    public bool IsMaxLevel
    {
        get
        {
            return null == GetNextLevelData();
        }
    }

    public long LevelUpCost
    {
        get
        {
            PlayerLevelData nextLevelData = GetNextLevelData();

            return null == nextLevelData ? 0 : (long)nextLevelData.UpgradeCost;
        }
    }

    public bool CanLevelUp
    {
        get
        {
            if (IsMaxLevel)
            {
                return false;
            }

            return GameManager.Session.Currency.DreamFragment >= LevelUpCost;
        }
    }

    public LevelUpResult TryLevelUp()
    {
        if (null == _ownedPlayerData)
        {
            return LevelUpResult.Error;
        }

        PlayerLevelData nextLevelData = GetNextLevelData();

        if (null == nextLevelData)
        {
            return LevelUpResult.MaxLevel;
        }

        if (!GameManager.Session.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost))
        {
            return LevelUpResult.NotEnoughCurrency;
        }

        _ownedPlayerData.Level += 1;

        Recalculate();

        return LevelUpResult.Success;
    }

    private PlayerLevelData GetNextLevelData()
    {
        if (null == _ownedPlayerData)
        {
            return null;
        }

        return GameManager.DataTable.GetPlayerLevelData(_ownedPlayerData.Level + 1);
    }

    public void Dispose()
    {
        if (null != _equiped)
        {
            _equiped.PropertyChanged -= OnEquipedChanged;
        }

        if (null != _equipment)
        {
            _equipment.ContainerPropertyChanged -= OnEquipmentChanged;
        }

        _ownedPlayerData = null;
        _equiped = null;
        _equipment = null;
    }

    private void OnEquipedChanged(object sender, PropertyChangedEventArgs e)
    {
        Recalculate();
    }

    private void OnEquipmentChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, HeroEquipmentState state)
    {
        Recalculate();
    }

    /// <summary>
    /// 기본 스텟 + 레벨 보너스 + 착용 장비 스텟을 다시 합산
    /// </summary>
    private void Recalculate()
    {
        StatSum baseStat = GetBaseStat();

        baseStat.Add(GetLevelBonus());

        StatSum equipmentStat = GetEquipmentStat(EquipmentType.Weapon);

        equipmentStat.Add(GetEquipmentStat(EquipmentType.Armor));
        equipmentStat.Add(GetEquipmentStat(EquipmentType.Accessory));

        StatSum sum = baseStat;

        sum.Add(equipmentStat);

        Level = null == _ownedPlayerData ? 0 : _ownedPlayerData.Level;

        BaseStat = baseStat;
        EquipmentStat = equipmentStat;

        Attack = sum.Attack;
        Hp = sum.Hp;
        Defense = sum.Defense;
        CriticalChance = sum.CriticalChance;
        BasicAttackHaste = sum.BasicAttackHaste;
        BasicActiveSkillHaste = sum.SignatureSkillHaste;

        CombatPower = CombatPowerCalculator.Calculate(this);
    }

    private StatSum GetBaseStat()
    {
        StatSum sum = new StatSum();

        PlayerData playerData = GameManager.DataTable.GetPlayerData(CharacterId.PLAYER_DATA);

        if (null == playerData)
        {
            Logger.LogError($"플레이어 기본 데이터를 찾을 수 없습니다. Id : {CharacterId.PLAYER_DATA}");
            return sum;
        }

        sum.Attack = playerData.BaseAttack;
        sum.Hp = playerData.BaseHp;
        sum.Defense = playerData.BaseDefense;
        sum.CriticalChance = playerData.BaseCritRate * Const.PERCENT_TO_RATE;
        sum.BasicAttackHaste = playerData.BaseNormalSkillHaste;
        sum.SignatureSkillHaste = playerData.BaseSpecialSkillHaste;

        return sum;
    }

    private StatSum GetLevelBonus()
    {
        StatSum sum = new StatSum();

        if (null == _ownedPlayerData)
        {
            return sum;
        }

        for (int level = Const.FIRST_BONUS_LEVEL; level <= _ownedPlayerData.Level; level++)
        {
            PlayerLevelData levelData = GameManager.DataTable.GetPlayerLevelData(level);

            if (null == levelData)
            {
                Logger.LogError($"플레이어 레벨 데이터를 찾을 수 없습니다. 레벨 : {level}");
                continue;
            }

            sum.Attack += levelData.BonusAttack;
            sum.Hp += levelData.BonusHP;
            sum.Defense += levelData.BonusDefense;
            sum.CriticalChance += levelData.BonusCriticalRate;
        }

        return sum;
    }

    private StatSum GetEquipmentStat(EquipmentType type)
    {
        StatSum sum = new StatSum();

        string equipmentId = _equiped.GetEquipedId(type);

        if (string.IsNullOrEmpty(equipmentId))
        {
            return sum;
        }

        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);

        if (null == equipmentData)
        {
            Logger.LogError($"장비 데이터가 없습니다. 장비 : {equipmentId}");
            return sum;
        }

        EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(
            Utils.GetEquipmentLevelDataId(equipmentData.Grade, _equipment.GetLevel(equipmentId)));

        if (null == levelData)
        {
            Logger.LogError($"장비 레벨 데이터가 없습니다. 장비 : {equipmentId}");
            return sum;
        }

        float multiplier = levelData.StatMultiplier;

        sum.Attack = equipmentData.BaseAttack * multiplier;
        sum.Hp = equipmentData.BaseHp * multiplier;
        sum.Defense = equipmentData.BaseDefense * multiplier;

        sum.CriticalChance = equipmentData.BaseCriticalChance * multiplier;
        sum.BasicAttackHaste = equipmentData.BasicAttackHaste * multiplier;
        sum.SignatureSkillHaste = equipmentData.SignatureSkillHaste * multiplier;

        return sum;
    }
}
