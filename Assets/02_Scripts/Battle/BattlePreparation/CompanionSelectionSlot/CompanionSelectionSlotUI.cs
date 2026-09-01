using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelectionSlotUI : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _slotButton;
    [SerializeField] private Image _slotImage;

    [Header("Stats")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _combatPowerText;

    private string _companionId;

    public event Action<string> SlotClicked;

    private void Awake()
    {
        UnityUtility.ValidateReference(_slotButton, nameof(_slotButton));
        UnityUtility.ValidateReference(_slotImage, nameof(_slotImage));
        UnityUtility.ValidateReference(_levelText, nameof(_levelText));
        UnityUtility.ValidateReference(_combatPowerText, nameof(_combatPowerText));
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
        if (companionId == null)
        {
            Logger.LogError($"'{companionId}'가 null입니다.");
            return;
        }

        _companionId = companionId;

        UpdateStatesText();
        UpdateSlotSprite();
    }

    public void Clear()
    {
        _companionId = null;
        _slotImage.sprite = null;
        _levelText.text = string.Empty;
        _combatPowerText.text = string.Empty;
    }

    private void UpdateStatesText()
    {
        CompanionState companionState = GameManager.Session.Companion.GetCompanion(_companionId);

        UpdateLevelText(companionState.Level);
        UpdateCompatPowerText(companionState.CombatPower);
    }

    private void UpdateLevelText(int level)
    {
        _levelText.text = $"Lv.{level}";
    }

    private void UpdateCompatPowerText(float combatPower)
    {
        int combatPowerValue = (int)combatPower;
        _combatPowerText.text = combatPowerValue.ToString();
    }

    private void UpdateSlotSprite()
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(_companionId);

        if (companionData == null)
        {
            Logger.LogError($"'{_companionId}' 동료 데이터를 찾을 수 없습니다.");
            return;
        }

        Sprite slotSprite = GameManager.Resource.GetLoadedAsset<Sprite>(companionData.SlotSpriteAddressableKey);

        if (slotSprite == null)
        {
            Logger.LogError($"'{_companionId}' 슬롯 스프라이트를 로드하지 못했습니다.");
            return;
        }

        _slotImage.sprite = slotSprite;
    }

    private void HandleSlotClicked()
    {
        SlotClicked?.Invoke(_companionId);
    }
}