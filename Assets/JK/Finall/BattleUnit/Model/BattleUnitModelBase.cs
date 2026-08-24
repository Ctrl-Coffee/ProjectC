using UnityEngine;

public abstract class BattleUnitModelBase : ModelBase
{
    protected float _maxHp;
    protected float _hp;
    protected float _attack;
    protected float _defense;
    protected float _criticalChance;
    protected float _criticalDamageMultiplier;

    private float _attackSpeed;
    private float _cooldownReduction;

    private string _basicAttackSkillId;
    private string _activeSkillId;

    private float _baseBasicAttackSkillCooldown;
    private float _baseActiveSkillCooldown;

    private float _calculatedBasicAttackSkillCooldown;
    private float _calculatedActiveSkillCooldown;

    private bool _isBasicAttackSkillReady;
    private bool _isActiveSkillReady;

    private bool _isDead;

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

    public bool IsActiveSkillReady
    {
        get { return _isActiveSkillReady; }
        private set
        {
            if (_isActiveSkillReady == value) { return; }

            _isActiveSkillReady = value;
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
            UpdateActiveSkillCooldown();
        }
    }

    public void Initialize(BattleUnitData battleUnitData)
    {
        InitializeStats(battleUnitData);
        InitializeSkills(battleUnitData);
        InitializeSkillCooldown();
        InitializeOnce();
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Hp));
        OnPropertyChanged(nameof(IsBasicAttackSkillReady));
        OnPropertyChanged(nameof(IsActiveSkillReady));
    }

    public void OnBattleStarted()
    {
        BasicAttackSkillCooldown();
        ActiveSkillCooldown();
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

    public void UseActiveSkill(int battlePosition)
    {
        if (!IsActiveSkillReady)
        {
            return;
        }

        IsActiveSkillReady = false;

        UseSkill(battlePosition, _activeSkillId);

        ActiveSkillCooldown();
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
    }

    public void Heal(float amount)
    {
        Hp += amount;
    }

    private void InitializeStats(BattleUnitData battleUnitData)
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

    private void InitializeSkills(BattleUnitData battleUnitData)
    {
        _basicAttackSkillId = battleUnitData.BasicAttackSkillId;
        _activeSkillId = battleUnitData.ActiveSkillId;
    }

    private void InitializeSkillCooldown()
    {
        SkillData basicAttackSkillData = GameManager.DataTable.GetSkillData(_basicAttackSkillId);
        SkillData activeSkillData = GameManager.DataTable.GetSkillData(_activeSkillId);

        if (basicAttackSkillData == null)
        {
            Debug.LogError($"'{_basicAttackSkillId}' 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        if (activeSkillData == null)
        {
            Debug.LogError($"'{_activeSkillId}' 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        _baseBasicAttackSkillCooldown = basicAttackSkillData.CoolTime;
        _baseActiveSkillCooldown = activeSkillData.CoolTime;

        UpdateBasicAttackSkillCooldown();
        UpdateActiveSkillCooldown();

        _isBasicAttackSkillReady = _calculatedBasicAttackSkillCooldown <= 0f;
        _isActiveSkillReady = _calculatedActiveSkillCooldown <= 0f;
    }

    public void Clear()
    {
        Hp = 0;
        Debug.Log("초기화");
    }

    private void BasicAttackSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown(_basicAttackSkillId, _calculatedBasicAttackSkillCooldown, OnBasicAttackSkillCooldownCompleted);
    }

    private void ActiveSkillCooldown()
    {
        TimeManagerTemp.Instance.RequestStartCooldown(_activeSkillId, _calculatedActiveSkillCooldown, OnActiveSkillCooldownCompleted);
    }

    private void OnBasicAttackSkillCooldownCompleted()
    {
        IsBasicAttackSkillReady = true;
    }

    private void OnActiveSkillCooldownCompleted()
    {
        IsActiveSkillReady = true;
    }

    private void UpdateBasicAttackSkillCooldown()
    {
        _calculatedBasicAttackSkillCooldown = BattleUtility.CalculateBasicAttackSkillCooldown(_baseBasicAttackSkillCooldown, _attackSpeed);
    }

    private void UpdateActiveSkillCooldown()
    {
        _calculatedActiveSkillCooldown = BattleUtility.CalculateActiveSkillCooldown(_baseActiveSkillCooldown, _cooldownReduction);
    }
    
    protected abstract void UseSkill(int battlePosition, string skillId);
}