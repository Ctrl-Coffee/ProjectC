using System;
using System.ComponentModel;
using UnityEngine;

public class BattleUnitStatusViewModel
{
    private BattleUnitModelBase _battleUnitModelBase;

    public event Action<string> PropertyChanged;

    public string Id
    {
        get { return _battleUnitModelBase.Id; }
    }

    public int BattlePosition
    {
        get { return _battleUnitModelBase.BattlePosition; }
    }

    public float MaxHp
    {
        get { return _battleUnitModelBase.MaxHp; }
    }

    public float Hp
    {
        get { return _battleUnitModelBase.Hp; }
    }

    public bool IsSignatureSkillReady
    {
        get { return _battleUnitModelBase.IsSignatureSkillReady; }
    }

    public float CalculatedSignatureSkillCooldown
    {
        get { return _battleUnitModelBase.CalculatedSignatureSkillCooldown; }
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

    public void UseSignatureSkill()
    {
        _battleUnitModelBase.UseSignatureSkill();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}
