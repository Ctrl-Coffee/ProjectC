using DG.Tweening;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Transform[] _destination;
    [SerializeField] private float _moveSpeed = 3f;

    private int _currentDestinationIndex;

    private Animator _anim;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        GameManager.Time.RequestStartCooldown("PlayerStay", GetSatyTime(), () =>
        {
            OnMove();
        });
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
        _anim.SetTrigger("isMove");

        transform.DOMove(destination.position, _moveSpeed)
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
