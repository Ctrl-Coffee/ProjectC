using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TestUnit : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _verticalRandomOffset = 0.3f;

    private Vector3 _initialPosition;
    private Tween _moveTween;

    public void Initialize()
    {
        _initialPosition = transform.position;
    }

    public void PlayIdle()
    {
        _animator.Play("Idle");
    }

    public void PlayWalk()
    {
        _animator.Play("Walk");
    }

    public void PlayAttack()
    {
        _animator.Play("Attack");
    }

    public void PlayHit()
    {
        _animator.Play("Hit");
    }

    public void PlayDeath()
    {
        _animator.Play("Death");
    }

    public async UniTask MoveTo(
        Vector3 targetPosition,
        float duration)
    {
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
        _moveTween?.Kill();
        _moveTween = null;

        Vector3 position = _initialPosition;

        position.y += Random.Range(-_verticalRandomOffset, _verticalRandomOffset);

        transform.position = position;
    }

    public void Stop()
    {
        _moveTween?.Kill();
        _moveTween = null;
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }
}
