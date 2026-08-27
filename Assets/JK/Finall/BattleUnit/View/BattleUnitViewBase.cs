using Cysharp.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;
public static class BehaviorGraphVariableNames
{
    public const string SkillReadyEvent = "SkillReadyEvent";
    public const string IsBasicAttackSkillReady = "IsBasicAttackSkillReady";
    public const string IsSignatureSkillReady = "IsSignatureSkillReady";
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class BattleUnitViewBase : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent _behaviorGraphAgent;
   
    private BlackboardVariable<SkillReadyEvent> _skillReadyEvent;
    private BlackboardVariable<bool> _isBasicAttackSkillReady;
    private BlackboardVariable<bool> _isSignatureSkillReady;

    private BattleUnitAnimator _battleUnitAnimator;
    private BattleUnitViewModel _battleUnitViewModel;

    public int BattlePosition
    {
        get { return _battleUnitViewModel.BattlePosition; }
    }

    public bool IsIdle
    {
        get { return _battleUnitAnimator.IsIdle; }
    }

    private void Awake()
    {
        UnityUtility.ValidateReference(_behaviorGraphAgent, nameof(_behaviorGraphAgent));

        CacheBehaviorVariables();

        Animator animator = GetComponent<Animator>();

        _battleUnitAnimator = new BattleUnitAnimator(animator);
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

    public void Initialize(BattleUnitModelBase baseBattleUnitModel)
    {
        InitializeViewModel(baseBattleUnitModel);
    }

    public void StartBattle()
    {
        if (!_battleUnitViewModel.IsInitialized)
        {
            _battleUnitViewModel.RequestSetActive(false);
            return;
        }

        _behaviorGraphAgent.Restart();
        _battleUnitViewModel.EnterBattle();
    }

    public void EndBattle()
    {
        _battleUnitViewModel.ExitBattle();
        _behaviorGraphAgent.End();
    }

    public void UseBasicAttackSkill()
    {
        if (_battleUnitViewModel.IsDead)
        {
            return;
        }

        if (!_battleUnitViewModel.RequestCheckBasicAttackSkillUsable())
        {
            return;
        }


        _battleUnitAnimator.Play(BattleUnitAnimationType.BasicAttack);
    }

    public void UseSignatureSkill()
    {
        if (_battleUnitViewModel.IsDead)
        {
            return;
        }

        if (!_battleUnitViewModel.RequestCheckSignatureSkillUsable())
        {
            return;
        }

        _battleUnitAnimator.Play(BattleUnitAnimationType.Signature);
    }

    public void OnBasicAttackAction()
    {
        _battleUnitViewModel.RequestUseBasicAttackSkill(_battleUnitViewModel.BattlePosition);
    }

    public void OnSignatureAction()
    {
        _battleUnitViewModel.RequestUseSignatureSkill(_battleUnitViewModel.BattlePosition);
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

    private void UpdateAnimation(string addressableKey)
    {
        _battleUnitAnimator.ApplyAnimationSet(addressableKey);
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
            Death().Forget();
        }
    }

    private async UniTask Death()
    {
        _battleUnitAnimator.Play(BattleUnitAnimationType.Death);

        await UniTask.WaitUntil(_battleUnitAnimator.IsDeathAnimationCompleted);

        EndBattle();

        _battleUnitViewModel.RequestSetActive(false);
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
                UpdateAnimation(_battleUnitViewModel.AnimKey);
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