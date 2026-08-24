using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelectionSlotUI : MonoBehaviour
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

    //TODO
    public void SetSprite(string spriteKey)
    {
        UIUtility.SetSpriteAsync(_companionPortraitImage, spriteKey).Forget();
    }

    public void OnSlotClicked()
    {
        SlotClicked?.Invoke(_companionDataId);
    }
}