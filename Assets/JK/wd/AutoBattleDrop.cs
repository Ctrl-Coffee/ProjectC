using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AutoBattleDrop : MonoBehaviour
{
    [SerializeField] private Image _image;

    [Header("튀어오르기")]
    [SerializeField] private float _scatterRadius = 60f;
    [SerializeField] private float _scatterDuration = 0.3f;
    [SerializeField] private float _startScale = 0.4f;

    [Header("날아가기")]
    [SerializeField] private float _holdDuration = 0.2f;
    [SerializeField] private float _flyDuration = 0.5f;
    [SerializeField] private float _arriveScale = 0.5f;

    private Sequence _sequence;
    private CurrencyType _currencyType;
    private Action<CurrencyType> _onArrived;

    public void Play(Sprite icon, CurrencyType currencyType, Vector3 startPosition, RectTransform target, float delay, Action<CurrencyType> onArrived)
    {
        _currencyType = currencyType;

        _onArrived = onArrived;

        if (null == _image)
        {
            _image = GetComponent<Image>();
        }

        if (null == _image)
        {
            Destroy(gameObject);
            return;
        }

        RectTransform rect = (RectTransform)transform;

        _image.raycastTarget = false;
        _image.sprite = icon;

        Color color = _image.color;
        color.a = 1f;
        _image.color = color;

        rect.position = startPosition;
        rect.localScale = Vector3.one * _startScale;

        float angle = UnityEngine.Random.Range(45f, 135f) * Mathf.Deg2Rad;

        Vector3 scatterPosition = startPosition
            + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _scatterRadius;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.AppendInterval(delay);

        _sequence.Append(rect.DOMove(scatterPosition, _scatterDuration).SetEase(Ease.OutQuad));
        _sequence.Join(rect.DOScale(1f, _scatterDuration).SetEase(Ease.OutBack));

        _sequence.AppendInterval(_holdDuration);

        _sequence.Append(rect.DOMove(target.position, _flyDuration).SetEase(Ease.InBack));
        _sequence.Join(rect.DOScale(_arriveScale, _flyDuration).SetEase(Ease.InQuad));

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
