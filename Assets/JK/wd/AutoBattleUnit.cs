using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AutoBattleUnit : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("피격 연출")]
    [SerializeField] private Color _hitColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private float _hitFlashDuration = 0.07f;
    [SerializeField] private float _hitKnockbackDistance = 0.15f;
    [SerializeField] private float _hitKnockbackDuration = 0.15f;

    private Color _originalColor = Color.white;
    private Tween _moveTween;
    private Tween _knockbackTween;
    private Tween _flashTween;
    private bool _isInitialized;

    private AnimatorOverrideController _overrideController;
    private readonly Dictionary<string, AnimationClip> _defaultClips = new Dictionary<string, AnimationClip>();

    private UniTaskCompletionSource _attackHitSource;

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        if (null == _spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (null != _spriteRenderer)
        {
            _originalColor = _spriteRenderer.color;
        }

        _isInitialized = true;
    }

    public void ApplyAnimationSet(UnitAnimationSet animationSet)
    {
        if (false == TryPrepareOverrideController())
        {
            return;
        }

        ApplyClip(Const.IDLE, null == animationSet ? null : animationSet.IdleClip);
        ApplyClip(Const.BASIC_ATTACK, null == animationSet ? null : animationSet.BasicAttackClip);
        ApplyClip(Const.SIGNATURE, null == animationSet ? null : animationSet.SignatureClip);
        ApplyClip(Const.DEATH, null == animationSet ? null : animationSet.DeathClip);
    }

    private bool TryPrepareOverrideController()
    {
        if (null != _overrideController)
        {
            return true;
        }

        if (null == _animator || null == _animator.runtimeAnimatorController)
        {
            return false;
        }

        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;

        CacheDefaultClip(Const.IDLE);
        CacheDefaultClip(Const.BASIC_ATTACK);
        CacheDefaultClip(Const.SIGNATURE);
        CacheDefaultClip(Const.DEATH);

        return true;
    }

    private void CacheDefaultClip(string clipName)
    {
        _defaultClips[clipName] = _overrideController[clipName];
    }

    private void ApplyClip(string clipName, AnimationClip animationClip)
    {
        if (null == animationClip)
        {
            _defaultClips.TryGetValue(clipName, out animationClip);
        }

        if (null == animationClip)
        {
            return;
        }

        _overrideController[clipName] = animationClip;
    }

    public void PlayIdle()
    {
        PlayState(Const.IDLE);
    }

    public void PlayRun()
    {
        PlayState(Const.RUN);
    }

    private void PlayState(string stateName)
    {
        if (null == _animator)
        {
            return;
        }

        _animator.ResetTrigger(Const.BASIC_ATTACK);
        _animator.ResetTrigger(Const.DEATH);

        _animator.Play(stateName, Const.BASE_LAYER, 0f);

        if (_animator.isActiveAndEnabled)
        {
            _animator.Update(0f);
        }
    }

    public async UniTask WaitForAttackHitAsync(float fallbackSeconds, CancellationToken cancellationToken)
    {
        _attackHitSource?.TrySetCanceled();
        _attackHitSource = new UniTaskCompletionSource();

        UniTask fallback = UniTask.Delay(Mathf.RoundToInt(fallbackSeconds * 1000f), cancellationToken: cancellationToken);

        await UniTask.WhenAny(_attackHitSource.Task, fallback);

        _attackHitSource = null;
    }

    public int GetSortingOrder()
    {
        if (null == _spriteRenderer)
        {
            return 0;
        }

        return _spriteRenderer.sortingOrder;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (null == _spriteRenderer)
        {
            return;
        }

        _spriteRenderer.sortingOrder = sortingOrder;
    }

    public float GetGroundY()
    {
        if (null == _spriteRenderer)
        {
            return transform.position.y;
        }

        return _spriteRenderer.bounds.min.y;
    }

    public Vector3 GetDropPosition()
    {
        if (null == _spriteRenderer)
        {
            return transform.position;
        }

        Bounds bounds = _spriteRenderer.bounds;

        return new Vector3(bounds.center.x, bounds.min.y, transform.position.z);
    }

    public float GetDeathAnimationLength()
    {
        if (null == _overrideController)
        {
            return 0f;
        }

        AnimationClip deathClip = _overrideController[Const.DEATH];

        if (null == deathClip)
        {
            return 0f;
        }

        return deathClip.length;
    }

    public void PlayAttack()
    {
        SetTrigger(Const.BASIC_ATTACK);
    }

    public void PlayDeath()
    {
        SetTrigger(Const.DEATH);
    }

    public void OnBasicAttackAction()
    {
        _attackHitSource?.TrySetResult();
    }

    public void OnSignatureAction()
    {
    }

    public void PlayHit()
    {
        PlayHitFlash();
        PlayHitKnockback();
    }

    private void PlayHitFlash()
    {
        if (null == _spriteRenderer)
        {
            return;
        }

        _flashTween?.Kill();

        _spriteRenderer.color = _originalColor;

        _flashTween = _spriteRenderer
            .DOColor(_hitColor, _hitFlashDuration)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void PlayHitKnockback()
    {
        _knockbackTween?.Kill(true);

        _knockbackTween = transform
            .DOPunchPosition(Vector3.right * _hitKnockbackDistance, _hitKnockbackDuration, 1, 0f);
    }

    private void SetTrigger(string triggerName)
    {
        if (null == _animator)
        {
            return;
        }

        _animator.SetTrigger(triggerName);
    }

    public async UniTask MoveTo(
        Vector3 targetPosition,
        float duration)
    {
        _knockbackTween?.Kill(true);
        _moveTween?.Kill();

        _moveTween = transform
            .DOMove(targetPosition, duration)
            .SetEase(Ease.Linear);

        await _moveTween.AsyncWaitForCompletion();
    }

    public void PlaceOnGround(float x, float groundY)
    {
        KillTweens();

        Vector3 position = transform.position;
        position.x = x;
        position.y = groundY;

        transform.position = position;

        if (null != _spriteRenderer)
        {
            float bottomGap = transform.position.y - _spriteRenderer.bounds.min.y;

            position.y = groundY + bottomGap;

            transform.position = position;

            _spriteRenderer.color = _originalColor;
        }
    }

    public void Stop()
    {
        KillTweens();
    }

    private void KillTweens()
    {
        _moveTween?.Kill();
        _moveTween = null;

        _knockbackTween?.Kill();
        _knockbackTween = null;

        _flashTween?.Kill();
        _flashTween = null;
    }

    private void OnDestroy()
    {
        KillTweens();
    }
}
