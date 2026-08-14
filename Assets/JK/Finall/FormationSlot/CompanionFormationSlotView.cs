using System;
using UnityEngine;

public sealed class CompanionFormationSlotView : FormationSlotView
{
    [SerializeField] private UIButtonComponent _slotButton;

    private int _slotIndex;

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

    public void Initialize(int slotIndex)
    {
        _slotIndex = slotIndex;
    }

    public void SetSelected(bool selected)
    {
        //선택 되었을 때 실행될 메서드
    }

    private void OnSlotClicked()
    {
        SlotClicked?.Invoke(_slotIndex);
    }
}