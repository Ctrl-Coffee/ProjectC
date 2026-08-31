using UnityEngine;

public interface ICurrencyEffectSource
{
    CurrencyType CurrencyType { get; }

    RectTransform Icon { get; }

    Sprite IconSprite { get; }

    bool IsVisible { get; }
    int IconCount { get; }
}
