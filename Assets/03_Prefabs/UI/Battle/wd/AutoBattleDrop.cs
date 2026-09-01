using DG.Tweening;
using System;
using UnityEngine;

public class AutoBattleDrop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("튀어오르기")]
    [SerializeField] private float _scatterRadius = 0.6f;
    [SerializeField] private float _scatterDuration = 0.3f;
    [SerializeField] private float _startScale = 0.4f;

    [Header("빨려들어가기")]
    [SerializeField] private float _holdDuration = 0.2f;
    [SerializeField] private float _flyDuration = 0.4f;
    [SerializeField] private float _arriveScale = 0.2f;

    private Sequence _sequence;
    private CurrencyType _currencyType;
    private Action<CurrencyType> _onArrived;

    public void Play(Sprite icon, CurrencyType currencyType, Vector3 startPosition, Vector3 targetPosition, int sortingOrder, float delay, Action<CurrencyType> onArrived)
    {
        _currencyType = currencyType;
        _onArrived = onArrived;

        if (null == _spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (null == _spriteRenderer)
        {
            Destroy(gameObject);
            return;
        }

        _spriteRenderer.sprite = icon;
        _spriteRenderer.sortingOrder = sortingOrder;

        transform.position = startPosition;
        transform.localScale = Vector3.one * _startScale;

        float angle = UnityEngine.Random.Range(45f, 135f) * Mathf.Deg2Rad;

        Vector3 scatterPosition = startPosition + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _scatterRadius;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.AppendInterval(delay);

        _sequence.Append(transform.DOMove(scatterPosition, _scatterDuration).SetEase(Ease.OutQuad));
        _sequence.Join(transform.DOScale(1f, _scatterDuration).SetEase(Ease.OutBack));

        _sequence.AppendInterval(_holdDuration);

        _sequence.Append(transform.DOMove(targetPosition, _flyDuration).SetEase(Ease.InBack));
        _sequence.Join(transform.DOScale(_arriveScale, _flyDuration).SetEase(Ease.InQuad));

        _sequence.OnComplete(OnPlayCompleted);
    }

    private void OnPlayCompleted()
    {
        Action<CurrencyType> callback = _onArrived;
        _onArrived = null;

        if (null != callback)
        {
            callback(_currencyType);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }
}
