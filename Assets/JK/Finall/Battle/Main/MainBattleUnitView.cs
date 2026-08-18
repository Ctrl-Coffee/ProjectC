public class MainBattleUnitView : BaseBattleUnitView
{
    public float Hp;

    private MainBattleUnitViewModel _mainBattleViewModel;

    public override bool IsDead => _mainBattleViewModel.Hp >= 0;

    protected override void Awake()
    {
        base.Awake();
        _mainBattleViewModel = new MainBattleUnitViewModel();
    }

    private void OnEnable()
    {
        _mainBattleViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _mainBattleViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
        _mainBattleViewModel.Dispose();
        _mainBattleViewModel = null;
    }

    protected override void BaseAttack()
    {
        //BattleManager.Instance.Re
    }

    protected override void UseSkill()
    {
        throw new System.NotImplementedException();
    }

    protected override void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeViewModel(string dataId)
    {
        _mainBattleViewModel.Initialize(dataId);
    }

    protected override void InitializeAnimation(string dataId)
    {
        //CompanionData companionData = GameManager.DataTable.GetCompanionData(dataId);
        //_battleUnitAnimationController.InitializeAnimation("");
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_mainBattleViewModel.Hp):
                Hp = _mainBattleViewModel.Hp;
                break;
        }
    }
}