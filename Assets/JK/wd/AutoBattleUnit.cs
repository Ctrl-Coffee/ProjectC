using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AutoBattleUnit : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _verticalRandomOffset = 0.3f;

    [Header("피격 연출")]
    [SerializeField] private Color _hitColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private float _hitFlashDuration = 0.07f;
    [SerializeField] private float _hitKnockbackDistance = 0.15f;
    [SerializeField] private float _hitKnockbackDuration = 0.15f;

    private Vector3 _initialPosition;
    private Color _originalColor = Color.white;
    private Tween _moveTween;
    private Tween _knockbackTween;
    private Tween _flashTween;
    private bool _isInitialized;

    private AnimatorOverrideController _overrideController;
    private readonly Dictionary<string, AnimationClip> _defaultClips = new Dictionary<string, AnimationClip>();

    // OnBasicAttackAction 이 완료시킨다.
    private UniTaskCompletionSource _attackHitSource;

    // 매번 갱신하면 이동 도중 꺼졌다 켜진 위치가 원점이 되어버린다.
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _initialPosition = transform.position;

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

        // 클립이 없는데 덮어쓰면 해당 상태가 통째로 비어버린다.
        if (null == animationClip)
        {
            return;
        }

        _overrideController[clipName] = animationClip;
    }

    // 트리거가 남아 있으면 재등장할 때 곧바로 재발동한다.
    public void PlayIdle()
    {
        if (null == _animator)
        {
            return;
        }

        _animator.ResetTrigger(Const.BASIC_ATTACK);
        _animator.ResetTrigger(Const.DEATH);

        _animator.Play(Const.IDLE, Const.BASE_LAYER, 0f);

        // SetActive 직후에는 Play 만으로 다음 프레임까지 이전 포즈가 남는다.
        if (_animator.isActiveAndEnabled)
        {
            _animator.Update(0f);
        }
    }

    // AnimationEvent 가 오지 않으면 fallbackSeconds 뒤에 그냥 진행한다.
    public async UniTask WaitForAttackHitAsync(float fallbackSeconds, CancellationToken cancellationToken)
    {
        _attackHitSource?.TrySetCanceled();
        _attackHitSource = new UniTaskCompletionSource();

        UniTask fallback = UniTask.Delay(Mathf.RoundToInt(fallbackSeconds * 1000f), cancellationToken: cancellationToken);

        await UniTask.WhenAny(_attackHitSource.Task, fallback);

        _attackHitSource = null;
    }

    public float GetDeathAnimationLength()
    {
        if (null == _overrideController)
        {
            return 0f;
        }

        AnimationClip deathClip = _overrideController[Const.DEATH];

        return null == deathClip ? 0f : deathClip.length;
    }

    public void PlayAttack()
    {
        SetTrigger(Const.BASIC_ATTACK);
    }

    public void PlayDeath()
    {
        SetTrigger(Const.DEATH);
    }

    // Hero_Attack 의 AnimationEvent 수신부. 지우면 "has no receiver" 경고가 뜬다.
    public void OnBasicAttackAction()
    {
        _attackHitSource?.TrySetResult();
    }

    // Hero_ActiveSkill 의 AnimationEvent 수신부. 위와 같다.
    public void OnSignatureAction()
    {
    }

    // 피격 클립이 없어 트윈으로 대체한다.
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
        // 완료 처리로 죽여야 펀치 이전 위치로 돌아간다.
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

    public async UniTask MoveLeft(
        float distance,
        float duration)
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x -= distance;

        await MoveTo(targetPosition, duration);
    }

    public void ResetPosition()
    {
        KillTweens();

        Vector3 position = _initialPosition;

        position.y += Random.Range(-_verticalRandomOffset, _verticalRandomOffset);

        transform.position = position;

        // 피격 도중 처치됐을 수 있어 색을 되돌린다.
        if (null != _spriteRenderer)
        {
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
