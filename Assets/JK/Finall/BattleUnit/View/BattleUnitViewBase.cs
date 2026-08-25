using Unity.Behavior;
using UnityEngine;
public static class BehaviorGraphVariableNames
{
    public const string SkillReadyEvent = "SkillReadyEvent";
    public const string IsBasicAttackSkillReady = "IsBasicAttackSkillReady";
    public const string IsSignatureSkillReady = "IsSignatureSkillReady";
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class BattleUnitViewBase : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent _behaviorGraphAgent;
   
    private BlackboardVariable<SkillReadyEvent> _skillReadyEvent;
    private BlackboardVariable<bool> _isBasicAttackSkillReady;
    private BlackboardVariable<bool> _isSignatureSkillReady;

    private int _battlePosition;

    private BattleUnitAnimationController _battleUnitAnimationController;
    private BattleUnitViewModel _battleUnitViewModel;

    public float HP;

    public int BattlePosition
    {
        get { return _battlePosition; }
    }

    private void Awake()
    {
        UnityUtility.ValidateReference(_behaviorGraphAgent, nameof(_behaviorGraphAgent));

        CacheBehaviorVariables();

        _battleUnitAnimationController = new BattleUnitAnimationController(GetComponent<Animator>());
        _battleUnitViewModel = new BattleUnitViewModel();
    }

    private void OnEnable()
    {
        _battleUnitViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _battleUnitViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
        _battleUnitViewModel.Dispose();
    }

    public void Initialize(int battlePosition, BattleUnitModelBase baseBattleUnitModel)
    {
        _battlePosition = battlePosition;
        InitializeViewModel(baseBattleUnitModel);
    }

    public void StartBattle()
    {
        _behaviorGraphAgent.enabled = true;
        _battleUnitViewModel.StartBattle();
    }

    public void EndBattle()
    {
        // _battleUnitViewModel.EndBattle();
        _behaviorGraphAgent.enabled = false;
    }

    public void UseBasicAttackSkill()
    {
        _battleUnitViewModel.RequestUseBasicAttackSkill(_battlePosition);
        //_battleUnitAnimationController.SetState(UnitState.Idle);
    }

    public void UseSignatureSkill()
    {
        _battleUnitViewModel.RequestUseSignatureSkill(_battlePosition);
        //_battleUnitAnimationController.SetState(UnitState.Idle);
    }

    private void CacheBehaviorVariables()
    {
        if (!_behaviorGraphAgent.GetVariable(BehaviorGraphVariableNames.SkillReadyEvent, out _skillReadyEvent))
        {
            Debug.LogError($"[BT] {BehaviorGraphVariableNames.SkillReadyEvent} 변수를 찾을 수 없습니다.");
        }

        if (!_behaviorGraphAgent.GetVariable(BehaviorGraphVariableNames.IsBasicAttackSkillReady, out _isBasicAttackSkillReady))
        {
            Debug.LogError($"[BT] {BehaviorGraphVariableNames.IsBasicAttackSkillReady} 변수를 찾을 수 없습니다.");
        }

        if (!_behaviorGraphAgent.GetVariable(BehaviorGraphVariableNames.IsSignatureSkillReady, out _isSignatureSkillReady))
        {
            Debug.LogError($"[BT] {BehaviorGraphVariableNames.IsSignatureSkillReady} 변수를 찾을 수 없습니다.");
        }
    }

    private void InitializeViewModel(BattleUnitModelBase baseBattleUnitModel)
    {
        _battleUnitViewModel.Initialize(baseBattleUnitModel);
    }

    private void UpdateAnim(string animKey)
    {
        _battleUnitAnimationController.InitializeAnimation(animKey);
    }

    private void UpdateHpBar(float hp)
    {
        Debug.Log($"체력 변경 {hp}");
        HP = hp;
    }

    private void UpdateBasicAttackSkillReady(bool isReady)
    {
        _isBasicAttackSkillReady.Value = isReady;

        if (isReady)
        {
            NotifySkillReadyStateChanged(UnitSkillType.BasicAttack);
        }
    }

    private void UpdateSignatureSkillReady(bool isReady)
    {
        _isSignatureSkillReady.Value = isReady;

        if (isReady)
        {
            NotifySkillReadyStateChanged(UnitSkillType.Signature);
        }
    }

    private void UpdateActiveState(bool isDead)
    {
        if (isDead)
        {
            Debug.Log($"{gameObject.name} 사망");
        }

        //gameObject.SetActive(!isDead);
    }

    private void NotifySkillReadyStateChanged(UnitSkillType unitSkillType)
    {
        _skillReadyEvent.Value.SendEventMessage(unitSkillType);
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_battleUnitViewModel.AnimKey):
                UpdateAnim(_battleUnitViewModel.AnimKey);
                break;
            case nameof(_battleUnitViewModel.Hp):
                UpdateHpBar(_battleUnitViewModel.Hp);
                break;
            case nameof(_battleUnitViewModel.IsBasicAttackSkillReady):
                UpdateBasicAttackSkillReady(_battleUnitViewModel.IsBasicAttackSkillReady);
                break;
            case nameof(_battleUnitViewModel.IsSignatureSkillReady):
                UpdateSignatureSkillReady(_battleUnitViewModel.IsSignatureSkillReady);
                break;
            case nameof(_battleUnitViewModel.IsDead):
                UpdateActiveState(_battleUnitViewModel.IsDead);
                break;
        }
    }
}