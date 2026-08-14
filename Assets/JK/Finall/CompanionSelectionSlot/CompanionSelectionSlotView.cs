using System;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelectionSlotView : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _slotButton;
    [SerializeField] private Image _companionPortraitImage;
    
    private string _companionDataId;

    public event Action<string> SlotClicked;

    private void Awake()
    {
        UnityUtility.ValidateReference(_slotButton, nameof(_slotButton));
        UnityUtility.ValidateReference(_companionPortraitImage, nameof(_companionPortraitImage));
    }

    private void OnEnable()
    {
        _slotButton.BindButtonEvent(OnSlotClicked);
    }

    private void OnDisable()
    {
        _slotButton.UnBindButtonAllEvent();
    }

    public void Initialize(string companionDataId)
    {
        _companionDataId = companionDataId;
    }

    public void SetSprite(Sprite sprite)
    {
        _companionPortraitImage.sprite = sprite;
    }

    public void OnSlotClicked()
    {
        SlotClicked?.Invoke(_companionDataId);
    }
}