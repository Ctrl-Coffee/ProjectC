using System;
using System.ComponentModel;

public class MainBattleUnitViewModel
{
    private readonly MainBattleUnitModel _mainBattleModel;

    public event Action<string> PropertyChanged;

    public MainBattleUnitViewModel()
    {
        _mainBattleModel = new MainBattleUnitModel();
        _mainBattleModel.PropertyChanged += OnPropertyChanged;
    }

    public float Hp
    {
        get { return _mainBattleModel.Hp; }
    }

    public void Dispose()
    {
        _mainBattleModel.PropertyChanged -= OnPropertyChanged;
    }

    public void Initialize(string dataId)
    {
        _mainBattleModel.Initialize(dataId);
    }

    public void Refresh()
    {
        _mainBattleModel.InitializeOnce();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}