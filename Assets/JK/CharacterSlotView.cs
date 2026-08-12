using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotView : MonoBehaviour
{
    [SerializeField] UIButtonComponent buttonComponent;

    ColorData _colorData;

    public event Action<ColorData> CharacterClicked;

    [SerializeField] private Image _image;

    private void OnEnable()
    {
        buttonComponent.BindButtonEvent(OnCharacterClicked);
    }

    private void OnDisable()
    {
        buttonComponent.UnBindButtonAllEvent();
    }

    public void Initialize(ColorData colorData, Action<ColorData> onCharacterClicked)
    {
        _colorData = colorData;
        CharacterClicked = onCharacterClicked;

        _image.color = colorData.Color;
    }

    public void OnCharacterClicked()
    {
        CharacterClicked?.Invoke(_colorData);
    }
}
