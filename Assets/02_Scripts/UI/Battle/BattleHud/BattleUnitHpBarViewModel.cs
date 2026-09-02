using System;
using System.ComponentModel;
using UnityEngine;

public class BattleUnitHpBarViewModel
{
    private BattleUnitModelBase _battleUnitModelBase;

    public event Action<string> PropertyChanged;

    public float MaxHp
    {
        get { return _battleUnitModelBase.MaxHp; }
    }

    public float Hp
    {
        get { return _battleUnitModelBase.Hp; }
    }

    public void SetModel(BattleUnitModelBase battleUnitModel)
    {
        if (battleUnitModel == null)
        {
            Debug.LogError("BattleUnitModel이 null입니다.");
            return;
        }

        if (_battleUnitModelBase != null)
        {
            _battleUnitModelBase.PropertyChanged -= OnPropertyChanged;
        }

        _battleUnitModelBase = battleUnitModel;
        _battleUnitModelBase.PropertyChanged += OnPropertyChanged;
    }

    public void Dispose()
    {
        if (_battleUnitModelBase == null)
        {
            return;
        }

        _battleUnitModelBase.PropertyChanged -= OnPropertyChanged;
        _battleUnitModelBase = null;
    }

    public void Refresh()
    {
        _battleUnitModelBase.InitializeOnce();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}