using System;
using UnityEngine;

public sealed class CompanionFormationSlotUI : FormationSlotUI
{
    private const int INVALID_SLOT_INDEX = -1;

    [SerializeField] private UIButtonComponent _slotButton;

    public int SlotIndex { get; private set; } = INVALID_SLOT_INDEX;

    public event Action<int> SlotClicked;

    protected override void Awake()
    {
        base.Awake();
        UnityUtility.ValidateReference(_slotButton, nameof(_slotButton));
    }

    private void OnEnable()
    {
        _slotButton.BindButtonEvent(OnSlotClicked);
    }

    private void OnDisable()
    {
        _slotButton.UnBindButtonAllEvent();
    }

    public void SetSlotIndex(int slotIndex)
    {
        SlotIndex = slotIndex;
    }

    public void ClearSlotIndex()
    {
        SlotIndex = INVALID_SLOT_INDEX;
    }

    public void SetSelected(bool selected)
    {
        //선택 되었을 때 실행될 메서드
    }

    private void OnSlotClicked()
    {
        SlotClicked?.Invoke(SlotIndex);
    }
}