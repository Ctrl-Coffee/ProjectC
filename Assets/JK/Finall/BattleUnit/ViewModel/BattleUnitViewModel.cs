using System;
using System.ComponentModel;
using UnityEngine;

public class BattleUnitViewModel
{
    private BattleUnitModelBase _battleUnitModelBase;

    public event Action<string> PropertyChanged;

    public int BattlePosition
    {
        get { return _battleUnitModelBase.BattlePosition; }
    }


    public bool IsInitialized
    {
        get { return _battleUnitModelBase.IsInitialized; }
    }

    public string AnimKey
    {
        get { return _battleUnitModelBase.AnimKey; }
    }

    public float Hp
    {
        get { return _battleUnitModelBase.Hp; }
    }

    public bool IsBasicAttackSkillReady
    {
        get { return _battleUnitModelBase.IsBasicAttackSkillReady; }
    }

    public bool IsSignatureSkillReady
    {
        get { return _battleUnitModelBase.IsSignatureSkillReady; }
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

    public void EnterBattle()
    {
        _battleUnitModelBase.EnterBattle();
    }

    public void ExitBattle()
    {
        _battleUnitModelBase.ExitBattle();
    }

    public bool RequestCheckBasicAttackSkillUsable()
    {
        bool isUsable = _battleUnitModelBase.CheckBasicAttackSkillUsable();
        return isUsable;
    }

    public bool RequestCheckSignatureSkillUsable()
    {
        bool isUsable = _battleUnitModelBase.CheckSignatureSkillUsable();
        return isUsable;
    }

    public void RequestUseBasicAttackSkill(int battlePosition)
    {
        _battleUnitModelBase.UseBasicAttackSkill(battlePosition);
    }

    public void RequestUseSignatureSkill(int battlePosition)
    {
        _battleUnitModelBase.UseSignatureSkill(battlePosition);
    }

    public void RequestSetActive(bool isActive)
    {
        _battleUnitModelBase.SetActive(isActive);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(e.PropertyName);
    }
}