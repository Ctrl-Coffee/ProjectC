using System;
using System.ComponentModel;
using UnityEngine;

public class BattleUnitViewModel
{
    private BattleUnitModelBase _battleUnitModelBase;

    public event Action<string> PropertyChanged;

    public float Hp
    {
        get { return _battleUnitModelBase.Hp; }
    }

    public bool IsBasicAttackSkillReady
    {
        get { return _battleUnitModelBase.IsBasicAttackSkillReady; }
    }

    public bool IsActiveSkillReady
    {
        get { return _battleUnitModelBase.IsActiveSkillReady; }
    }

    public bool IsDead
    {
        get { return _battleUnitModelBase.IsDead; }
    }

    public void Initialize(BattleUnitModelBase battleUnitModel)
    {
        if (battleUnitModel == null)
        {
            Debug.LogError("BattleUnitModel이 null입니다.");
            return;
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

    public void RequestUseBasicAttackSkill(int battlePosition)
    {
        _battleUnitModelBase.UseBasicAttackSkill(battlePosition);
    }

    public void RequestUseActiveSkill(int battlePosition)
    {
        _battleUnitModelBase.UseActiveSkill(battlePosition);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}