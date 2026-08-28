using System.ComponentModel;

public class HeroInfoModel : ModelBase, IStatData
{
    private struct StatSum
    {
        public float Attack;
        public float Hp;
        public float Defense;
        public float CriticalRate;
        public float NormalSkillHaste;
        public float SpecialSkillHaste;

        public void Add(StatSum other)
        {
            Attack += other.Attack;
            Hp += other.Hp;
            Defense += other.Defense;
            CriticalRate += other.CriticalRate;
            NormalSkillHaste += other.NormalSkillHaste;
            SpecialSkillHaste += other.SpecialSkillHaste;
        }
    }

    private PlayerGrowthModel _growth;
    private HeroEquipedModel _equiped;
    private HeroEquipmentModel _equipment;

    private float _attack;
    private float _hp;
    private float _defense;
    private float _criticalRate;
    private float _normalSkillHaste;
    private float _specialSkillHaste;
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

    public float CriticalRate
    {
        get { return _criticalRate; }
        private set
        {
            if (_criticalRate == value) return;
            _criticalRate = value;
            OnPropertyChanged();
        }
    }

    public float NormalSkillHaste
    {
        get { return _normalSkillHaste; }
        private set
        {
            if (_normalSkillHaste == value) return;
            _normalSkillHaste = value;
            OnPropertyChanged();
        }
    }

    public float SpecialSkillHaste
    {
        get { return _specialSkillHaste; }
        private set
        {
            if (_specialSkillHaste == value) return;
            _specialSkillHaste = value;
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

    public HeroInfoModel(PlayerGrowthModel growth, HeroEquipedModel equiped, HeroEquipmentModel equipment)
    {
        _growth = growth;
        _equiped = equiped;
        _equipment = equipment;

        // 레벨업 / 장비 착용 / 장비 강화 시 최종 스텟이 바뀌므로 구독
        _growth.PropertyChanged += OnGrowthChanged;
        _equiped.PropertyChanged += OnEquipedChanged;
        _equipment.ContainerPropertyChanged += OnEquipmentChanged;

        Recalculate();
    }

    public override void InitializeOnce()
    {
        Recalculate();
    }

    public void Dispose()
    {
        _growth.PropertyChanged -= OnGrowthChanged;
        _equiped.PropertyChanged -= OnEquipedChanged;
        _equipment.ContainerPropertyChanged -= OnEquipmentChanged;

        _growth = null;
        _equiped = null;
        _equipment = null;
    }

    private void OnGrowthChanged(object sender, PropertyChangedEventArgs e)
    {
        Recalculate();
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
    public void Recalculate()
    {
        StatSum sum = GetBaseStat();

        sum.Add(GetLevelBonus());
        sum.Add(GetEquipmentStat(EquipmentType.Weapon));
        sum.Add(GetEquipmentStat(EquipmentType.Armor));
        sum.Add(GetEquipmentStat(EquipmentType.Accessory));

        Level = null == _growth ? 0 : _growth.Level;

        Attack = sum.Attack;
        Hp = sum.Hp;
        Defense = sum.Defense;
        CriticalRate = sum.CriticalRate;
        NormalSkillHaste = sum.NormalSkillHaste;
        SpecialSkillHaste = sum.SpecialSkillHaste;

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
        sum.CriticalRate = playerData.BaseCritRate * Const.PERCENT_TO_RATE;
        sum.NormalSkillHaste = playerData.BaseNormalSkillHaste;
        sum.SpecialSkillHaste = playerData.BaseSpecialSkillHaste;

        return sum;
    }

    private StatSum GetLevelBonus()
    {
        StatSum sum = new StatSum();

        for (int level = Const.FIRST_BONUS_LEVEL; level <= _growth.Level; level++)
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
            sum.CriticalRate += levelData.BonusCriticalRate;
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

        return sum;
    }
}
