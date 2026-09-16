using DG.Tweening;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform[] _destination;
    [SerializeField] private float _moveSpeed = 3f;

    private int _currentDestinationIndex;

    private Animator _anim;
    private SpriteRenderer _spriteRenderer; 
    private Tween _moveTween;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localPosition = transform.parent.InverseTransformPoint(_destination[0].position);
    }

    private void OnEnable()
    {
        Stay();
    }

    private void OnDisable()
    {
        GameManager.Time.RequestCancelCooldown("PlayerStay");

        _moveTween?.Kill();
        _moveTween = null;
    }

    private int GetSatyTime()
    {
        return Random.Range(1, 4);
    }

    private void OnMove()
    {
        var destPosIndex = GetRandomDestinationIndex();

        if(destPosIndex == _currentDestinationIndex)
        {
            Stay();
            return;
        }
        else if(destPosIndex < _currentDestinationIndex)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
        _currentDestinationIndex = destPosIndex;

        Transform destination = _destination[_currentDestinationIndex];
        Vector3 destinationLocalPosition = transform.parent.InverseTransformPoint(destination.position);

        _anim.SetTrigger("isMove");

        _moveTween = transform.DOLocalMove(destinationLocalPosition, _moveSpeed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(Stay);
    }

    private void Stay()
    {
        _anim.SetTrigger("isIdle");

        GameManager.Time.RequestStartCooldown("PlayerStay", GetSatyTime(), () =>
        {
            OnMove();
        });
    }

    private int GetRandomDestinationIndex()
    {
        return Random.Range(0, _destination.Length);
    }
}
