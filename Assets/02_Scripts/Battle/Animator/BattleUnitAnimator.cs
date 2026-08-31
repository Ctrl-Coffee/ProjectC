using System.Collections.Generic;
using UnityEngine;

public class BattleUnitAnimator
{
    private readonly Animator _animator;
    private readonly AnimatorOverrideController _animatorOverrideController;

    private readonly Dictionary<BattleUnitAnimationType, int> _animationHashes = new Dictionary<BattleUnitAnimationType, int>();
    private readonly Dictionary<string, AnimationClip> _defaultClips = new Dictionary<string, AnimationClip>();

    public bool IsIdle
    {
        get
        {
            return IsCurrentState(BattleUnitAnimationType.Idle);
        }
    }

    public BattleUnitAnimator(Animator animator)
    {
        if (animator == null)
        {
            Logger.LogError($"'{nameof(BattleUnitAnimator)}' Animator가 Null입니다.");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {
            Logger.LogError($"'{nameof(BattleUnitAnimator)}' RuntimeAnimatorController가 Null입니다.");
            return;
        }

        _animator = animator;
        _animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = _animatorOverrideController;

        InitializeAnimationHashes();
        InitializeDefaultClips();
    }

    public void ApplyAnimationSet(string addressableKey)
    {
        if (string.IsNullOrWhiteSpace(addressableKey))
        {
            ResetToDefaultClips();
            return;
        }

        UnitAnimationSet unitAnimationSet = GameManager.Resource.GetLoadedAsset<UnitAnimationSet>(addressableKey);

        if (unitAnimationSet == null)
        {
            Logger.LogError($"'{addressableKey}' 애니메이션 세트를 로드하지 못했습니다.");
            ResetToDefaultClips();
            return;
        }

        ApplyAnimationClip(Const.IDLE, unitAnimationSet.IdleClip);
        ApplyAnimationClip(Const.BASIC_ATTACK, unitAnimationSet.BasicAttackClip);
        ApplyAnimationClip(Const.SIGNATURE, unitAnimationSet.SignatureClip);
        ApplyAnimationClip(Const.DEATH, unitAnimationSet.DeathClip);
    }

    public void Play(BattleUnitAnimationType animType)
    {
        if (!_animationHashes.TryGetValue(animType, out int animHash))
        {
            return;
        }

        if (animType == BattleUnitAnimationType.Idle)
        {
            _animator.Play(animHash, 0, 0f);
            return;
        }

        _animator.SetTrigger(animHash);
    }

    public bool IsDeathAnimationCompleted()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(Const.BASE_LAYER);

        bool isDeathState = stateInfo.shortNameHash == _animationHashes[BattleUnitAnimationType.Death];

        bool isAnimationCompleted = stateInfo.normalizedTime >= 1f;

        return isDeathState && isAnimationCompleted;
    }

    private void InitializeAnimationHashes()
    {
        _animationHashes[BattleUnitAnimationType.Idle] = Animator.StringToHash(Const.IDLE);
        _animationHashes[BattleUnitAnimationType.BasicAttack] = Animator.StringToHash(Const.BASIC_ATTACK);
        _animationHashes[BattleUnitAnimationType.Signature] = Animator.StringToHash(Const.SIGNATURE);
        _animationHashes[BattleUnitAnimationType.Death] = Animator.StringToHash(Const.DEATH);
    }

    private void InitializeDefaultClips()
    {
        _defaultClips[Const.IDLE] = _animatorOverrideController[Const.IDLE];
        _defaultClips[Const.BASIC_ATTACK] = _animatorOverrideController[Const.BASIC_ATTACK];
        _defaultClips[Const.SIGNATURE] = _animatorOverrideController[Const.SIGNATURE];
        _defaultClips[Const.DEATH] = _animatorOverrideController[Const.DEATH];
    }

    private void ApplyAnimationClip(string animationName, AnimationClip animationClip)
    {
        if (animationClip == null)
        {
            animationClip = _defaultClips[animationName];
        }

        _animatorOverrideController[animationName] = animationClip;
    }

    private void ResetToDefaultClips()
    {
        ApplyAnimationClip(Const.IDLE, null);
        ApplyAnimationClip(Const.BASIC_ATTACK, null);
        ApplyAnimationClip(Const.SIGNATURE, null);
        ApplyAnimationClip(Const.DEATH, null);
    }

    private bool IsCurrentState(BattleUnitAnimationType animType)
    {
        if (!_animationHashes.TryGetValue(animType, out int hash))
        {
            return false;
        }

        return _animator.GetCurrentAnimatorStateInfo(Const.BASE_LAYER).shortNameHash == hash;
    }
}