using System.ComponentModel;

public class PlayerStatViewModel : ViewModelBase<PlayerStatModel>
{
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

    public PlayerStatViewModel(PlayerStatModel model) : base(model)
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
        CriticalRate = _model.CriticalRate;
        NormalSkillHaste = _model.NormalSkillHaste;
        SpecialSkillHaste = _model.SpecialSkillHaste;

        NormalSkillCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(NormalSkillHaste);
        SpecialSkillCooldownReduceRate = SkillCooldownCalculator.GetCooldownReduceRate(SpecialSkillHaste);
        CombatPower = _model.CombatPower;
    }
}
