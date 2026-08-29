using System.ComponentModel;

public class HeroInfoViewModel : ViewModelBase<HeroInfoModel>
{
    public string Name { get; private set; }
    public int Level { get; private set; }

    public float Attack { get; private set; }
    public float Hp { get; private set; }
    public float Defense { get; private set; }
    public float CriticalChance { get; private set; }
    public float BasicAttackHaste { get; private set; }
    public float SignatureSkillHaste { get; private set; }
    public float BasicAttackCooldownReduceRate { get; private set; }
    public float SignatureSkillCooldownReduceRate { get; private set; }
    public float CombatPower { get; private set; }

    public StatSum BaseStat { get; private set; }
    public StatSum EquipmentStat { get; private set; }

    public long LevelUpCost { get; private set; }
    public bool IsMaxLevel { get; private set; }
    public bool CanLevelUp { get; private set; }

    public HeroInfoViewModel(HeroInfoModel model) : base(model)
    {
    }

    public override void InitializeModel()
    {
        RefreshStats();

        base.InitializeModel();
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        RefreshStats();

        base.OnPropertyChanged(sender, e);
    }

    private void RefreshStats()
    {
        PlayerData playerData = GameManager.DataTable.GetPlayerData(CharacterId.PLAYER_DATA);

        Name = null == playerData ? string.Empty : playerData.Name;
        Level = _model.Level;

        Attack = _model.Attack;
        Hp = _model.Hp;
        Defense = _model.Defense;
        CriticalChance = _model.CriticalChance;
        BasicAttackHaste = _model.BasicAttackHaste;
        SignatureSkillHaste = _model.SignatureSkillHaste;

        BaseStat = _model.BaseStat;
        EquipmentStat = _model.EquipmentStat;

        BasicAttackCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(BasicAttackHaste);
        SignatureSkillCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(SignatureSkillHaste);
        CombatPower = _model.CombatPower;

        RefreshLevelUpState();
    }

    public void RefreshLevelUpState()
    {
        LevelUpCost = _model.LevelUpCost;
        IsMaxLevel = _model.IsMaxLevel;
        CanLevelUp = _model.CanLevelUp;
    }

    public LevelUpResult TryLevelUp()
    {
        LevelUpResult result = _model.TryLevelUp();

        RefreshStats();

        return result;
    }
}
