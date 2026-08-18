public class CompanionBattleUnitView : BaseBattleUnitView
{
    public float Hp;

    private CompanionBattleUnitViewModel _companionBattleUnitViewModel;

    public override bool IsDead => _companionBattleUnitViewModel.Hp >= 0;

    protected override void Awake()
    {
        _companionBattleUnitViewModel = new CompanionBattleUnitViewModel();
    }

    private void OnEnable()
    {
        _companionBattleUnitViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _companionBattleUnitViewModel.PropertyChanged -= OnPropertyChanged;
    }

    protected  void OnDestroy()
    {
        _companionBattleUnitViewModel.Dispose();
        _companionBattleUnitViewModel = null;
    }
    protected override void BaseAttack()
    {
        throw new System.NotImplementedException();
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
        _companionBattleUnitViewModel.Initialize(dataId);
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
            case nameof(_companionBattleUnitViewModel.Hp):
                Hp = _companionBattleUnitViewModel.Hp;
                break;
        }
    }
}