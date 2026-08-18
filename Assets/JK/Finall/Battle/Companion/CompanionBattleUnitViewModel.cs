using System;
using System.ComponentModel;

public class CompanionBattleUnitViewModel
{
    private readonly CompanionBattleUnitModel _companionBattleUnitModel;

    public event Action<string> PropertyChanged;

    public CompanionBattleUnitViewModel()
    {
        _companionBattleUnitModel = new CompanionBattleUnitModel();
        _companionBattleUnitModel.PropertyChanged += OnPropertyChanged;
    }

    public float Hp
    {
        get { return _companionBattleUnitModel.Hp; }
    }

    public void Dispose()
    {
        _companionBattleUnitModel.PropertyChanged -= OnPropertyChanged;
    }

    public void Initialize(string dataId)
    {
        _companionBattleUnitModel.Initialize(dataId);
    }

    public void Refresh()
    {
        _companionBattleUnitModel.InitializeOnce();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}