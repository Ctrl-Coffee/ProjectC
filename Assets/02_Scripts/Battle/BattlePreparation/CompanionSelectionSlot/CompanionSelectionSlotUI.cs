using System;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelectionSlotUI : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _slotButton;
    [SerializeField] private Image _slotImage;

    private string _companionDataId;

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

    public void Initialize(string companionDataId)
    {
        _companionDataId = companionDataId;
        UpdateSlotSprite(companionDataId);
    }

    public void Clear()
    {
        _companionDataId = null;
        _slotImage.sprite = null;
    }

    private void UpdateSlotSprite(string companionDataId)
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionDataId);

        if (companionData == null)
        {
            Debug.LogError($"'{companionDataId}' 동료 데이터를 찾을 수 없습니다.");
            return;
        }

        // TODO: 동료 데이터 연동 후 실제 스프라이트 키 적용
        Sprite slotSprite = GameManager.Resource.GetLoadedAsset<Sprite>("TestSlotSprite");

        if (slotSprite == null)
        {
            Debug.LogError($"슬롯 스프라이트를 로드하지 못했습니다.");
            return;
        }

        _slotImage.sprite = slotSprite;
    }

    private void HandleSlotClicked()
    {
        SlotClicked?.Invoke(_companionDataId);
    }
}