using System;
using System.ComponentModel;

public class EnemyBattleUnitViewModel
{
    private readonly EnemyBattleUnitModel _enemyBattleUnitModel;

    public event Action<string> PropertyChanged;

    public EnemyBattleUnitViewModel()
    {
        _enemyBattleUnitModel = new EnemyBattleUnitModel();
        _enemyBattleUnitModel.PropertyChanged += OnPropertyChanged;
    }

    public float Hp
    {
        get { return _enemyBattleUnitModel.Hp; }
    }

    public void Dispose()
    {
        _enemyBattleUnitModel.PropertyChanged -= OnPropertyChanged;
    }

    public void Initialize(string dataId)
    {
        _enemyBattleUnitModel.Initialize(dataId);
    }

    public void Refresh()
    {
        _enemyBattleUnitModel.InitializeOnce();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}