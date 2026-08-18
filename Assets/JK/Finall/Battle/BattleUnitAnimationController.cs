using UnityEngine;

public class BattleUnitAnimationController
{
    private readonly Animator _animator;
    private AnimatorOverrideController _animatorOverrideController;

    private UnitState _currentUnitState;

    public BattleUnitAnimationController(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError("");
            return;
        }

        _animator = animator;
        //_animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        //_animator.runtimeAnimatorController = _animatorOverrideController;
    }

    public void InitializeAnimation(string key)
    {
        //TODO 나중에 데이터 연동시 할당
        //UnitAnimationSet unitAnimationSet = GameManager.Resource.GetLoadedAsset<UnitAnimationSet>(key);

        //_animatorOverrideController[""] = unitAnimationSet.Idle;
        //_animatorOverrideController[""] = unitAnimationSet.BasicAttack;
        //_animatorOverrideController["Attack"] = unitAnimationSet.Skill;
        //_animatorOverrideController["Hit"] = unitAnimationSet.Hit;
        //_animatorOverrideController["Dead"] = unitAnimationSet.Dead;
    }

    public void SetState(UnitState unitState)
    {
        if (_currentUnitState == unitState)
        {
            return;
        }

        _currentUnitState = unitState;

        switch (_currentUnitState)
        {
            case UnitState.BaseAttack:
                _animator.SetTrigger("");
                break;
            case UnitState.Skill:
                _animator.SetTrigger("");
                break;
            case UnitState.Hit:
                _animator.SetTrigger("");
                break;
            case UnitState.Dead:
                _animator.SetTrigger("");
                break;
        }
    }
}