using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CurrencyIconAnchor : MonoBehaviour
{
    [SerializeField] private CurrencyType _currencyType;

    private static List<CurrencyIconAnchor> _actives = new();

    private RectTransform _rect;

    public CurrencyType CurrencyType
    {
        get
        {
            return _currencyType;
        }
    }

    public RectTransform Rect
    {
        get
        {
            if (null == _rect)
            {
                _rect = (RectTransform)transform;
            }

            return _rect;
        }
    }

    public static CurrencyIconAnchor Find(CurrencyType currencyType)
    {
        for (int i = 0; i < _actives.Count; i++)
        {
            if (null != _actives[i] && _actives[i].CurrencyType == currencyType)
            {
                return _actives[i];
            }
        }

        return null;
    }

    public static CurrencyIconAnchor FindAny()
    {
        for (int i = 0; i < _actives.Count; i++)
        {
            if (null != _actives[i])
            {
                return _actives[i];
            }
        }

        return null;
    }

    private void OnEnable()
    {
        if (!_actives.Contains(this))
        {
            _actives.Add(this);
        }
    }

    private void OnDisable()
    {
        _actives.Remove(this);
    }
}
