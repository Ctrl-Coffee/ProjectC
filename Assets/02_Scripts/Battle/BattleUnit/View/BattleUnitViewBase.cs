using Cysharp.Threading.Tasks;
using System;
using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public abstract class BattleUnitViewBase : MonoBehaviour
{
    [SerializeField] protected BehaviorGraphAgent _behaviorGraphAgent;
   
    private BlackboardVariable<SkillReadyEvent> _skillReadyEvent;
    private BlackboardVariable<bool> _isBasicAttackSkillReady;
    private BlackboardVariable<bool> _isSignatureSkillReady;

    private BattleUnitAnimator _battleUnitAnimator;
    protected BattleUnitViewModel _battleUnitViewModel;

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

    protected virtual void OnEnable()
    {
        _battleUnitViewModel.PropertyChanged += OnPropertyChanged;
        _battleUnitViewModel.TakeDamage += HandleTakeDamage;
    }

    protected virtual void OnDisable()
    {
        _battleUnitViewModel.PropertyChanged -= OnPropertyChanged;
        _battleUnitViewModel.TakeDamage -= HandleTakeDamage;
    }

    private void OnDestroy()
    {
        _battleUnitViewModel.Dispose();
    }

    public void Initialize(BattleUnitModelBase baseBattleUnitModel)
    {
        _battleUnitViewModel.Initialize(baseBattleUnitModel);
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
        if (_battleUnitViewModel.IsDead)
        {
            return;
        }

        HandleBattleExit();
    }

    public void UseBasicAttackSkill()
    {
        if (_battleUnitViewModel.IsDead)
        {
            return;
        }

        if (!_battleUnitViewModel.IsBasicAttackSkillReady)
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

        if (!_battleUnitViewModel.IsSignatureSkillReady)
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
        _battleUnitViewModel.RequestUseBasicAttackSkill();
    }

    public void OnSignatureAction()
    {
        _battleUnitViewModel.RequestUseSignatureSkill();
    }

    protected virtual void CacheBehaviorVariables()
    {
        if (!_behaviorGraphAgent.GetVariable(Const.SKILL_READY_EVENT, out _skillReadyEvent))
        {
            Logger.LogError($"'{Const.SKILL_READY_EVENT}' 변수를 찾을 수 없습니다.");
        }

        if (!_behaviorGraphAgent.GetVariable(Const.BASIC_ATTACK_SKILL_READY, out _isBasicAttackSkillReady))
        {
            Logger.LogError($"'{Const.BASIC_ATTACK_SKILL_READY}' 변수를 찾을 수 없습니다.");
        }

        if (!_behaviorGraphAgent.GetVariable(Const.SIGNATURE_SKILL_READY, out _isSignatureSkillReady))
        {
            Logger.LogError($"'{Const.SIGNATURE_SKILL_READY}' 변수를 찾을 수 없습니다.");
        }
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

    protected void UpdateSignatureSkillReady(bool isReady)
    {
        _isSignatureSkillReady.Value = isReady;

        if (isReady)
        {
            NotifySkillReadyStateChanged(UnitSkillType.Signature);
        }
    }

    private async UniTask UpdateActiveState(bool isDead)
    {
        if (isDead)
        {
            await PlayDeathSequence();
        }

        _battleUnitViewModel.RequestSetActive(!isDead);
    }

    private async UniTask PlayDeathSequence()
    {
        _battleUnitAnimator.Play(BattleUnitAnimationType.Death);

        HandleBattleExit();

        try
        {
            await UniTask.WaitUntil(_battleUnitAnimator.IsDeathAnimationCompleted, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        catch(OperationCanceledException)
        {
            return;
        }
    }

    private void NotifySkillReadyStateChanged(UnitSkillType unitSkillType)
    {
        _skillReadyEvent.Value.SendEventMessage(unitSkillType);
    }

    private void HandleBattleExit()
    {
        _battleUnitViewModel.ExitBattle();
        _behaviorGraphAgent.End();
    }

    private void HandleTakeDamage(DamageResult damageResult)
    {
        GameManager.UI.ShowDamageText(damageResult, transform.position);
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
                UpdateActiveState(_battleUnitViewModel.IsDead).Forget();
                break;
        }
    }
}