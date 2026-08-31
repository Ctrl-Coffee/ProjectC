using System;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelectionSlotUI : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _slotButton;
    [SerializeField] private Image _slotImage;

    private string _companionId;

    public event Action<string> SlotClicked;

    private void Awake()
    {
        UnityUtility.ValidateReference(_slotButton, nameof(_slotButton));
        UnityUtility.ValidateReference(_slotImage, nameof(_slotImage));
    }

    private void OnEnable()
    {
        _slotButton.BindButtonEvent(HandleSlotClicked);
    }

    private void OnDisable()
    {
        _slotButton.UnBindButtonAllEvent();
    }

    public void Initialize(string companionId)
    {
        _companionId = companionId;
        UpdateSlotSprite(companionId);
    }

    public void Clear()
    {
        _companionId = null;
        _slotImage.sprite = null;
    }

    private void UpdateSlotSprite(string companionId)
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionId);

        if (companionData == null)
        {
            Logger.LogError($"'{companionId}' 동료 데이터를 찾을 수 없습니다.");
            return;
        }

        Sprite slotSprite = GameManager.Resource.GetLoadedAsset<Sprite>(companionData.SlotSpriteAddressableKey);

        if (slotSprite == null)
        {
            Logger.LogError($"'{companionId}' 슬롯 스프라이트를 로드하지 못했습니다.");
            return;
        }

        _slotImage.sprite = slotSprite;
    }

    private void HandleSlotClicked()
    {
        SlotClicked?.Invoke(_companionId);
    }
}