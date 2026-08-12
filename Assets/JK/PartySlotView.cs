using System;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotView : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _slotButton;

    private int _slotIndex;
    private bool _isSelected;

    public event Action<int> PartySlotClicked;

    [SerializeField] private Image _image;

    private void OnEnable()
    {
        _slotButton.BindButtonEvent(OnSlotClicked);
    }

    private void OnDisable()
    {
        _slotButton.UnBindAllButtonEvent();
    }

    public void Initialize(int slotIndex, Action<int> onSupportSlotClicked)
    {
        _slotIndex = slotIndex;
        PartySlotClicked = onSupportSlotClicked;
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
    }

    public bool HasCharacter()
    {
        bool hasCharacter = _image.color != Color.white;
        return hasCharacter;
    }

    private void OnSlotClicked()
    {
        PartySlotClicked?.Invoke(_slotIndex);
    }

    public void SetCharacter(ColorData colorData)
    {
        if (colorData == null)
        {
            _image.color = Color.white;
            return;
        }

        _image.color = colorData.Color;
    }
}