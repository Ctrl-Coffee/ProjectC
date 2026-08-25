using UnityEngine;

public abstract class BattleUnitModelBase : ModelBase
{
    private string _unitId;
    private string _animKey;

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

    private float _baseBasicAttackSkillCooldown;
    private float _baseSignatureSkillCooldown;

    private float _calculatedBasicAttackSkillCooldown;
    private float _calculatedsignatureSkillCooldown;

    private bool _isBasicAttackSkillReady;
    private bool _isSignatureSkillReady;

    private bool _isDead;

    public string AnimKey
    {
        get { return _animKey; }
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
        }
    }

    //공격속도 변경 로직 추가시 사용
    private float AttackSpeed
    {
        set
        {
            if (_attackSpeed == value) { return; }

            _attackSpeed = value;
            UpdateBasicAttackSkillCooldown();
        }
    }

    //쿨타임 감소 변경 로직 추가시 사용
    private float CooldownReduction
    {
        set
        {
            if (_cooldownReduction == value) { return; }

            _cooldownReduction = value;
            UpdateSignatureSkillCooldown();
        }
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

    public void StartBattle()
    {
        BasicAttackSkillCooldown();
        SignatureSkillCooldown();
    }

    public void UseBasicAttackSkill(int battlePosition)
    {
        if (!IsBasicAttackSkillReady)
        {
            return;
        }

        IsBasicAttackSkillReady = false;

        UseSkill(battlePosition, _basicAttackSkillId);

        BasicAttackSkillCooldown();
    }

    public void UseSignatureSkill(int battlePosition)
    {
        if (!IsSignatureSkillReady)
        {
            return;
        }

        IsSignatureSkillReady = false;

        UseSkill(battlePosition, _signatureSkillId);

        SignatureSkillCooldown();
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
    }

    public void Heal(float amount)
    {
        Hp += amount;
    }

    private void InitializeIdentity(BattleUnitData battleUnitData)
    {
        _unitId = battleUnitData.UnitId;
        _animKey = battleUnitData.Key;
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

        _baseBasicAttackSkillCooldown = basicAttackSkillData.CoolTime;
        _baseSignatureSkillCooldown = signatureSkillData.CoolTime;

        UpdateBasicAttackSkillCooldown();
        UpdateSignatureSkillCooldown();

        _isBasicAttackSkillReady = _calculatedBasicAttackSkillCooldown <= 0f;
        _isSignatureSkillReady = _calculatedsignatureSkillCooldown <= 0f;
    }

    public void Clear()
    {
        Hp = 0;
        Debug.Log("초기화");
    }

    private void BasicAttackSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown($"{_unitId}_{_basicAttackSkillId}", _calculatedBasicAttackSkillCooldown, OnBasicAttackSkillCooldownCompleted);
    }

    private void SignatureSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown($"{_unitId}_{_signatureSkillId}", _calculatedsignatureSkillCooldown, OnSignatureSkillCooldownCompleted);
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
        _calculatedBasicAttackSkillCooldown = BattleUtility.CalculateBasicAttackSkillCooldown(_baseBasicAttackSkillCooldown, _attackSpeed);
    }

    private void UpdateSignatureSkillCooldown()
    {
        _calculatedsignatureSkillCooldown = BattleUtility.CalculateSignatureSkillCooldown(_baseSignatureSkillCooldown, _cooldownReduction);
    }
    
    protected abstract void UseSkill(int battlePosition, string skillId);
}