using System;
using UnityEngine;

public abstract class BattleUnitModelBase : ModelBase
{
    private int _battlePosition;

    private string _uId;
    private string _animationSetKey;

    protected float _maxHp;
    protected float _hp;
    protected float _attack;
    protected float _defense;
    protected float _criticalChance;
    protected float _criticalDamageMultiplier;

    private float _attackSpeed;
    private float _cooldownReduction;

    private string _basicAttackSkillId;
    private string _signatureSkillId;

    private float _basicAttackSkillCooldown;
    private float _signatureSkillCooldown;

    private float _calculatedBasicAttackSkillCooldown;
    private float _calculatedsignatureSkillCooldown;

    private bool _isBasicAttackSkillReady;
    private bool _isSignatureSkillReady;

    private bool _isDead;

    private bool _isInitialized;

    public event Action<bool> DeadStateChanged;

    public int BattlePosition
    {
        get { return _battlePosition; }
    }

    public string AnimKey
    {
        get { return _animationSetKey; }
    }

    public float MaxHp
    {
        get { return _maxHp; }
    }

    public bool IsInitialized
    {
        get { return _isInitialized; }
    }

    public float Hp
    {
        get { return _hp; }
        private set
        {
            float clampedHp = Mathf.Clamp(value, 0f, _maxHp);

            if (_hp == clampedHp)
            {
                return;
            }

            _hp = clampedHp;

            OnPropertyChanged();

            IsDead = Hp <= 0;
        }
    }

    public bool IsBasicAttackSkillReady
    {
        get { return _isBasicAttackSkillReady; }
        private set
        {
            if (_isBasicAttackSkillReady == value) { return; }

            _isBasicAttackSkillReady = value;
            OnPropertyChanged();
        }
    }

    public bool IsSignatureSkillReady
    {
        get { return _isSignatureSkillReady; }
        private set
        {
            if (_isSignatureSkillReady == value) { return; }

            _isSignatureSkillReady = value;
            OnPropertyChanged();
        }
    }

    public bool IsDead
    {
        get { return _isDead; }
        private set
        {
            if (_isDead == value) { return; }

            _isDead = value;
            OnPropertyChanged();
            OnDeadStateChanged();
        }
    }

    public BattleUnitModelBase(int battlePosition)
    {
        _battlePosition = battlePosition;
    }

    public void Initialize(BattleUnitData battleUnitData)
    {
        InitializeIdentity(battleUnitData);
        InitializeUnitStats(battleUnitData);
        InitializeUnitSkills(battleUnitData);
        InitializeSkillCooldown();
        InitializeOnce();
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(AnimKey));
        OnPropertyChanged(nameof(Hp));
        OnPropertyChanged(nameof(IsBasicAttackSkillReady));
        OnPropertyChanged(nameof(IsSignatureSkillReady));
    }
    
    public void Clear()
    {
        ClearIdentity();
        ClearUnitStats();
        ClearUnitSkills();
        ClearSkillCooldowns();
        InitializeOnce();
    }

    public void EnterBattle()
    {
        BasicAttackSkillCooldown();
        SignatureSkillCooldown();
    }

    public void ExitBattle()
    {
        CancelBasicAttackSkillCooldown();
        CancelSignatureSkillCooldown();
    }

    public bool CheckBasicAttackSkillUsable()
    {
        if (!IsBasicAttackSkillReady)
        {
            return false;
        }

        bool isUsable = BattleManager.Instance.CheckPlayerSkillUsable(_basicAttackSkillId);
        return isUsable;
    }

    public bool CheckSignatureSkillUsable()
    {
        if (!IsSignatureSkillReady)
        {
            return false;
        }

        bool isUsable = BattleManager.Instance.CheckPlayerSkillUsable(_signatureSkillId);
        return isUsable;
    }

    public void UseBasicAttackSkill(int battlePosition)
    {
        IsBasicAttackSkillReady = false;
        UseSkill(battlePosition, _basicAttackSkillId);
        BasicAttackSkillCooldown();
    }

    public void UseSignatureSkill(int battlePosition)
    {
        IsSignatureSkillReady = false;
        UseSkill(battlePosition, _signatureSkillId);
        SignatureSkillCooldown();
    }

    public void ReceiveAttack(SkillExecutionData skillExecutionData)
    {
        DefenseStats defenseStats = new DefenseStats(_defense);

        float damage = BattleUtility.CalculateDamage(skillExecutionData, defenseStats);

        TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        Hp += amount;
    }

    private void InitializeIdentity(BattleUnitData battleUnitData)
    {
        _uId = battleUnitData.UId;
        _animationSetKey = battleUnitData.AnimationSetKey;
        _isInitialized = true;
    }

    private void InitializeUnitStats(BattleUnitData battleUnitData)
    {
        _maxHp = battleUnitData.MaxHp;
        _hp = _maxHp;
        _attack = battleUnitData.Attack;
        _defense = battleUnitData.Defense;
        _criticalChance = battleUnitData.CriticalChance;
        _criticalDamageMultiplier = battleUnitData.CriticalDamageMultiplier;
        _attackSpeed = battleUnitData.AttackSpeed;
        _cooldownReduction = battleUnitData.CooldownReduction;
        _isDead = false;
    }

    private void InitializeUnitSkills(BattleUnitData battleUnitData)
    {
        _basicAttackSkillId = battleUnitData.BasicAttackSkillId;
        _signatureSkillId = battleUnitData.SignatureSkillId;
    }

    private void InitializeSkillCooldown()
    {
        SkillData basicAttackSkillData = GameManager.DataTable.GetSkillData(_basicAttackSkillId);
        SkillData signatureSkillData = GameManager.DataTable.GetSkillData(_signatureSkillId);

        if (basicAttackSkillData == null)
        {
            Debug.LogError($"'{_basicAttackSkillId}' 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        if (signatureSkillData == null)
        {
            Debug.LogError($"'{_signatureSkillId}' 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        _basicAttackSkillCooldown = basicAttackSkillData.CoolTime;
        _signatureSkillCooldown = signatureSkillData.CoolTime;

        UpdateBasicAttackSkillCooldown();
        UpdateSignatureSkillCooldown();

        _isBasicAttackSkillReady = _calculatedBasicAttackSkillCooldown <= 0f;
        _isSignatureSkillReady = _calculatedsignatureSkillCooldown <= 0f;
    }

    private void ClearIdentity()
    {
        _uId = string.Empty;
        _animationSetKey = string.Empty;
        _isInitialized = false;
    }

    private void ClearUnitStats()
    {
        _maxHp = 0f;
        _hp = 0f;
        _attack = 0f;
        _defense = 0f;
        _criticalChance = 0f;
        _criticalDamageMultiplier = 0f;
        _attackSpeed = 0f;
        _cooldownReduction = 0f;
        _isDead = false;
    }

    private void ClearUnitSkills()
    {
        _basicAttackSkillId = string.Empty;
        _signatureSkillId = string.Empty;
    }

    private void ClearSkillCooldowns()
    {
        _basicAttackSkillCooldown = 0f;
        _signatureSkillCooldown = 0f;
        _calculatedBasicAttackSkillCooldown = 0f;
        _calculatedsignatureSkillCooldown = 0f;

        IsBasicAttackSkillReady = false;
        IsSignatureSkillReady = false;
    }

    private void BasicAttackSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown($"{_uId}_{_basicAttackSkillId}", _calculatedBasicAttackSkillCooldown, OnBasicAttackSkillCooldownCompleted);
    }

    private void SignatureSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown($"{_uId}_{_signatureSkillId}", _calculatedsignatureSkillCooldown, OnSignatureSkillCooldownCompleted);
    }

    private void CancelBasicAttackSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestCancelCooldown($"{_uId}_{_basicAttackSkillId}");
    }

    private void CancelSignatureSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestCancelCooldown($"{_uId}_{_signatureSkillId}");
    }

    private void OnBasicAttackSkillCooldownCompleted()
    {
        IsBasicAttackSkillReady = true;
    }

    private void OnSignatureSkillCooldownCompleted()
    {
        IsSignatureSkillReady = true;
    }

    private void UpdateBasicAttackSkillCooldown()
    {
        _calculatedBasicAttackSkillCooldown = BattleUtility.CalculateBasicAttackSkillCooldown(_basicAttackSkillCooldown, _attackSpeed);
    }

    private void UpdateSignatureSkillCooldown()
    {
        _calculatedsignatureSkillCooldown = BattleUtility.CalculateSignatureSkillCooldown(_signatureSkillCooldown, _cooldownReduction);
    }

    private void TakeDamage(float damage)
    {
        Hp -= damage;
    }

    private void OnDeadStateChanged()
    {
        if (DeadStateChanged == null)
        {
            return;
        }

        DeadStateChanged.Invoke(_isDead);
    }

    public abstract void SetActive(bool isActive);
    protected abstract void UseSkill(int battlePosition, string skillId);
}