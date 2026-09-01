using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyFlyEffect : MonoBehaviour
{
    private const int MAX_ICON_COUNT = 10;
    private const float ICON_SIZE = 56f;

    private const float SCATTER_RADIUS = 70f;
    private const float SCATTER_ANGLE_JITTER = 18f;

    private const float ICON_DELAY_STEP = 0.1f;
    private const float CURRENCY_DELAY_STEP = 0.18f;

    private const float PUNCH_SCALE = 0.3f;
    private const float PUNCH_DURATION = 0.3f;
    private const int PUNCH_VIBRATO = 6;
    private const float PUNCH_ELASTICITY = 0.6f;

    private static CurrencyFlyEffect _instance;

    private Dictionary<CurrencyType, int> _arrivedCounts = new();
    private Dictionary<CurrencyType, int> _totalCounts = new();

    private Action<CurrencyType, float> _onProgress;
    private Action _onCompleted;

    private int _flyingCount;

    public static CurrencyFlyEffect GetOrCreate()
    {
        if (null == _instance)
        {
            _instance = Create();
        }

        return _instance;
    }

    public void Play(IReadOnlyList<ICurrencyEffectSource> sources, Action<CurrencyType, float> onProgress, Action onCompleted)
    {
        Complete();

        _onProgress = onProgress;
        _onCompleted = onCompleted;
        _totalCounts.Clear();
        _arrivedCounts.Clear();
        _flyingCount = 0;

        transform.SetAsLastSibling();

        int currencyIndex = 0;
        
        for (int i = 0; null != sources && i < sources.Count; i++)
        {

            if (TryPlaySource(sources[i], currencyIndex * CURRENCY_DELAY_STEP))
            {
                currencyIndex++;
            }
        }

        if (0 == _flyingCount)
        {
            Complete();
        }
    }

    private static CurrencyFlyEffect Create()
    {
        Transform overlayRoot = FindOverlayRoot();

        if (null == overlayRoot)
        {
            return null;
        }

        GameObject effectObject = new GameObject(nameof(CurrencyFlyEffect), typeof(RectTransform), typeof(CurrencyFlyEffect));

        RectTransform rect = (RectTransform)effectObject.transform;

        rect.SetParent(overlayRoot, false);

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return effectObject.GetComponent<CurrencyFlyEffect>();
    }

    private static Transform FindOverlayRoot()
    {
        CurrencyIconAnchor anchor = CurrencyIconAnchor.FindAny();

        if (null == anchor)
        {
            return null;
        }

        UIManagerHelper helper = anchor.GetComponentInParent<UIManagerHelper>(true);

        if (null == helper || null == helper.Canvas)
        {
            return null;
        }

        int overlayIndex = (int)UIRootType.Overlay;

        if (helper.Canvas.Count <= overlayIndex)
        {
            return null;
        }

        return helper.Canvas[overlayIndex];
    }

    private bool TryPlaySource(ICurrencyEffectSource source, float currencyDelay)
    {
        if (null == source || !source.IsVisible || null == source.Icon)
        {
            return false;
        }

        CurrencyIconAnchor anchor = CurrencyIconAnchor.Find(source.CurrencyType);

        if (null == anchor)
        {
            Logger.LogWarning($"{source.CurrencyType} 도착 지점(CurrencyIconAnchor)이 없어 연출을 건너뜁니다.");
            return false;
        }

        int iconCount = Mathf.Clamp(source.IconCount, 1, MAX_ICON_COUNT);

        _totalCounts.TryGetValue(source.CurrencyType, out int total);
        _totalCounts[source.CurrencyType] = total + iconCount;

        for (int i = 0; i < iconCount; i++)
        {
            _flyingCount++;

            CreateIcon().Play(source, GetScatterOffset(i, iconCount), anchor.Rect, currencyDelay + i * ICON_DELAY_STEP, OnIconArrived);
        }

        return true;
    }

    private void OnIconArrived(CurrencyFlyIcon icon)
    {
        _flyingCount--;

        GameManager.Sound.PlaySFX(AddressablePath.Audio.CURRENCY_GAIN);

        PunchAnchor(icon.CurrencyType);

        _arrivedCounts.TryGetValue(icon.CurrencyType, out int arrived);

        arrived++;

        _arrivedCounts[icon.CurrencyType] = arrived;

        if (null != _onProgress)
        {
            _totalCounts.TryGetValue(icon.CurrencyType, out int total);
            _onProgress(icon.CurrencyType, (float)arrived / Mathf.Max(1, total));
        }

        if (0 < _flyingCount)
        {
            return;
        }

        Complete();
    }

    private void PunchAnchor(CurrencyType currencyType)
    {
        CurrencyIconAnchor anchor = CurrencyIconAnchor.Find(currencyType);

        if (null == anchor)
        {
            return;
        }

        anchor.Rect.DOKill(complete: true);
        anchor.Rect.localScale = Vector3.one;

        Tween tween = anchor.Rect.DOPunchScale(Vector3.one * PUNCH_SCALE, PUNCH_DURATION, PUNCH_VIBRATO, PUNCH_ELASTICITY);

        tween.SetUpdate(true);
    }

    private Vector3 GetScatterOffset(int index, int count)
    {
        float angle = 360f / count * index + UnityEngine.Random.Range(-SCATTER_ANGLE_JITTER, SCATTER_ANGLE_JITTER);
        float radian = angle * Mathf.Deg2Rad;
        float radius = SCATTER_RADIUS * UnityEngine.Random.Range(0.6f, 1f);

        return new Vector3(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius, 0f);
    }

    private CurrencyFlyIcon CreateIcon()
    {
        GameObject iconObject = new GameObject("CurrencyFlyIcon", typeof(RectTransform), typeof(Image), typeof(CurrencyFlyIcon));

        RectTransform rect = (RectTransform)iconObject.transform;

        rect.SetParent(transform, false);
        rect.sizeDelta = new Vector2(ICON_SIZE, ICON_SIZE);

        return iconObject.GetComponent<CurrencyFlyIcon>();
    }

    private void Complete()
    {
        Action callback = _onCompleted;

        _onCompleted = null;
        _onProgress = null;

        callback?.Invoke();
    }

    private void OnDestroy()
    {
        Complete();
    }
}
