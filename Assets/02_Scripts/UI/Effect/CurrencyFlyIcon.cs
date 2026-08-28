using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyFlyIcon : MonoBehaviour
{
    private const float SCATTER_DURATION = 0.25f;
    private const float FLY_DURATION = 0.5f;

    private const float START_SCALE = 0.4f;
    private const float ARRIVE_SCALE = 0.5f;

    private Action<CurrencyFlyIcon> _onArrived;

    private CurrencyType _currencyType;

    public CurrencyType CurrencyType
    {
        get
        {
            return _currencyType;
        }
    }

    public void Play(ICurrencyEffectSource source, Vector3 scatterOffset, RectTransform target, float delay, Action<CurrencyFlyIcon> onArrived)
    {
        RectTransform rect = (RectTransform)transform;
        Image image = GetComponent<Image>();

        _currencyType = source.CurrencyType;
        _onArrived = onArrived;

        image.raycastTarget = false;
        image.sprite = source.IconSprite;
        image.enabled = null != image.sprite;

        Vector3 start = source.Icon.position;

        rect.position = start;
        rect.localScale = Vector3.one * START_SCALE;

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.AppendInterval(delay);

        sequence.Append(rect.DOMove(start + scatterOffset, SCATTER_DURATION).SetEase(Ease.OutQuad));
        sequence.Join(rect.DOScale(1f, SCATTER_DURATION).SetEase(Ease.OutBack));

        sequence.Append(rect.DOMove(target.position, FLY_DURATION).SetEase(Ease.InBack));
        sequence.Join(rect.DOScale(ARRIVE_SCALE, FLY_DURATION).SetEase(Ease.InQuad));

        sequence.OnComplete(OnFlyCompleted);
    }

    private void OnFlyCompleted()
    {
        Action<CurrencyFlyIcon> callback = _onArrived;
        _onArrived = null;

        callback?.Invoke(this);

        Destroy(gameObject);
    }
}
