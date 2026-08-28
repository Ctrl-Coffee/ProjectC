using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            if (!_animationHashes.TryGetValue(BattleUnitAnimationType.Idle, out int idleHash))
            {
                return false;
            }

            return _animator.GetCurrentAnimatorStateInfo(Const.BASE_LAYER).shortNameHash == idleHash;
        }
    }

    public BattleUnitAnimator(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError($"'{nameof(BattleUnitAnimator)}' Animator가 Null입니다.");
            return;
        }

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        if (controller == null)
        {
            Debug.LogError($"'{nameof(BattleUnitAnimator)}' RuntimeAnimatorController가 Null입니다.");
            return;
        }

        _animator = animator;
        _animatorOverrideController = new AnimatorOverrideController(controller);

        animator.runtimeAnimatorController = _animatorOverrideController;

        InitializeAnimationHashes();
        InitializeDefaultClips();
    }

    public void ApplyAnimationSet(string addressableKey)
    {
        Init(addressableKey).Forget();
        //TODO 나중에 데이터 연동시 할당
        //UnitAnimationSet unitAnimationSet = GameManager.Resource.GetLoadedAsset<UnitAnimationSet>(key);

        //_animatorOverrideController[""] = unitAnimationSet.Idle;
        //_animatorOverrideController[""] = unitAnimationSet.BasicAttack;
        //_animatorOverrideController["Attack"] = unitAnimationSet.Skill;
        //_animatorOverrideController["Hit"] = unitAnimationSet.Hit;
        //_animatorOverrideController["Dead"] = unitAnimationSet.Dead;
    }

    public void Play(BattleUnitAnimationType animType)
    {
        if (animType == BattleUnitAnimationType.Idle)
        {
            return;
        }

        if (!_animationHashes.TryGetValue(animType, out int triggerHash))
        {
            return;
        }

        _animator.SetTrigger(triggerHash);
    }

    public bool IsDeathAnimationCompleted()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(Const.BASE_LAYER);

        bool isDeathState = stateInfo.shortNameHash == _animationHashes[BattleUnitAnimationType.Death];

        bool isAnimationCompleted = stateInfo.normalizedTime >= 1f;

        return isDeathState && isAnimationCompleted;
    }

    //TODO 임시
    private async UniTask Init(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            ApplyAnimationClip(Const.IDLE, null);
            ApplyAnimationClip(Const.BASIC_ATTACK, null);
            ApplyAnimationClip(Const.SIGNATURE, null);
            ApplyAnimationClip(Const.DEATH, null);
            return;
        }

        UnitAnimationSet unitAnimationSet = await Addressables.LoadAssetAsync<UnitAnimationSet>(id);

        ApplyAnimationClip(Const.IDLE, unitAnimationSet.IdleClip);
        ApplyAnimationClip(Const.BASIC_ATTACK, unitAnimationSet.BasicAttackClip);
        ApplyAnimationClip(Const.SIGNATURE, unitAnimationSet.SignatureClip);
        ApplyAnimationClip(Const.DEATH, unitAnimationSet.DeathClip);
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
}