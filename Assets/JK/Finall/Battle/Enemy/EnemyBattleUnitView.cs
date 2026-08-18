public class EnemyBattleUnitView : BaseBattleUnitView
{
    public float Hp;
    
    private EnemyBattleUnitViewModel _enemyBattleUnitViewModel;

    public override bool IsDead => _enemyBattleUnitViewModel.Hp >= 0;

    protected override void Awake()
    {
        _enemyBattleUnitViewModel = new EnemyBattleUnitViewModel();
    }

    private void OnEnable()
    {
        _enemyBattleUnitViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _enemyBattleUnitViewModel.PropertyChanged -= OnPropertyChanged;
    }

    protected  void OnDestroy()
    {
        _enemyBattleUnitViewModel.Dispose();
        _enemyBattleUnitViewModel = null;
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
        _enemyBattleUnitViewModel.Initialize(dataId);
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
            case nameof(_enemyBattleUnitViewModel.Hp):
                Hp = _enemyBattleUnitViewModel.Hp;
                break;
        }
    }
}