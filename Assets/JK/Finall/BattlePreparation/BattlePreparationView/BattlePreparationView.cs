using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattlePreparationView : MonoBehaviour
{
    private const int INVALID_COMPANION_POSITION = -1;

    [Header("Companion Selection")]
    [SerializeField] private Transform _companionSelectionContent;
    [SerializeField] private CompanionSelectionSlotUI _companionSelectionSlotPrefab;

    [Header("Start Battle")]
    [SerializeField] private UIButtonComponent _startBattleButton;

    private Camera _mainCamera;

    private int _selectedCompanionPosition = INVALID_COMPANION_POSITION;
    private string _selectedCompanionSelectionId;

    private readonly List<CompanionSelectionSlotUI> _companionSelectionSlotPool = new List<CompanionSelectionSlotUI>();

    private void Awake()
    {
        ValidateReference();

        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        RefreshCompanionSelectionSlots();

        _startBattleButton.BindButtonEvent(HandleStartBattleButtonClicked);
    }

    private void Update()
    {
        Pointer pointer = Pointer.current;

        if (pointer == null || !pointer.press.wasPressedThisFrame)
        {
            return;
        }

        if (!TryGetClickedCompanionPosition(out int position))
        {
            return;
        }

        HandleCompanionPositionClicked(position);
    }

    private void OnDisable()
    {
        _startBattleButton.UnBindButtonAllEvent();

        ClearSelectedCompanionPosition();
        ClearSelectedCompanionId();
    }

    private void OnDestroy()
    {
        foreach (CompanionSelectionSlotUI slot in _companionSelectionSlotPool)
        {
            if (slot == null)
            {
                continue;
            }

            slot.SlotClicked -= HandleCompanionSelectionSlotClicked;
        }
    }

    private void ValidateReference()
    {
        UnityUtility.ValidateReference(_companionSelectionContent, nameof(_companionSelectionContent));
        UnityUtility.ValidateReference(_companionSelectionSlotPrefab, nameof(_companionSelectionSlotPrefab));
        UnityUtility.ValidateReference(_startBattleButton, nameof(_startBattleButton));
    }

    private bool TryGetClickedCompanionPosition(out int position)
    {
        position = INVALID_COMPANION_POSITION;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

        if (hitCollider == null)
        {
            return false;
        }

        if (!hitCollider.TryGetComponent(out BattleUnitView companionView))
        {
            return false;
        }

        position = companionView.FormationSlotIndex;
        return true;
    }

    private void HandleCompanionPositionClicked(int position)
    {
        if (TrySetCompanionToSelectedPosition(position))
        {
            return;
        }

        if (TrySelectCompanionPosition(position))
        {
            return;
        }

        try
        {
            if (TrySwapCompanion(position))
            {
                return;
            }

            TryRemoveCompanion(position);
        }
        finally
        {
            ClearSelectedCompanionPosition();
        }
    }

    private bool TrySetCompanionToSelectedPosition(int position)
    {
        if (string.IsNullOrWhiteSpace(_selectedCompanionSelectionId))
        {
            return false;
        }

        bool isCompanionSet = BattleManager.Instance.RequestSetCompanionToPosition(position, _selectedCompanionSelectionId);

        if (isCompanionSet)
        {
            _selectedCompanionSelectionId = null;
        }

        return isCompanionSet;
    }

    private bool TrySelectCompanionPosition(int position)
    {
        if (_selectedCompanionPosition != INVALID_COMPANION_POSITION)
        {
            return false;
        }

        _selectedCompanionPosition = position;

        return true;
    }

    private bool TrySwapCompanion(int position)
    {
        if (_selectedCompanionPosition == position)
        {
            return false;
        }

        bool isCompanionSwapped = BattleManager.Instance.RequestSwapCompanion(_selectedCompanionPosition, position);
        return isCompanionSwapped;
    }

    private bool TryRemoveCompanion(int position)
    {
        bool isCompanionRemoved = BattleManager.Instance.RequestRemoveCompanion(position);
        return isCompanionRemoved;
    }

    private void ClearSelectedCompanionPosition()
    {
        _selectedCompanionPosition = INVALID_COMPANION_POSITION;
    }

    private void ClearSelectedCompanionId()
    {
        _selectedCompanionSelectionId = null;
    }

    private void RefreshCompanionSelectionSlots()
    {
        //TODO 현재 가진 동료 모델 리스트 가져오기
        Dictionary<string, CompanionData> companionDataTable = GameManager.DataTable.CompanionDataTable;

        int activeSlotIndex = 0;

        foreach (CompanionData companionData in companionDataTable.Values)
        {
            if (companionData == null)
            {
                continue;
            }

            CompanionSelectionSlotUI slot = GetCompanionSelectionSlot(activeSlotIndex);

            slot.Initialize(companionData.Id);
            slot.SetSprite(companionData.FormationSpriteKey);
            slot.gameObject.SetActive(true);

            activeSlotIndex++;
        }

        DisableUnusedCompanionSelectionSlots(activeSlotIndex);
    }

    private CompanionSelectionSlotUI GetCompanionSelectionSlot(int index)
    {
        if (index < _companionSelectionSlotPool.Count)
        {
            return _companionSelectionSlotPool[index];
        }

        CompanionSelectionSlotUI companionSelectionSlotUI = Instantiate(_companionSelectionSlotPrefab, _companionSelectionContent);

        companionSelectionSlotUI.SlotClicked += HandleCompanionSelectionSlotClicked;

        _companionSelectionSlotPool.Add(companionSelectionSlotUI);

        return companionSelectionSlotUI;
    }

    private void DisableUnusedCompanionSelectionSlots(int activeSlotCount)
    {
        for (int index = activeSlotCount; index < _companionSelectionSlotPool.Count; index++)
        {
            CompanionSelectionSlotUI companionSelectionSlotUI = _companionSelectionSlotPool[index];
            companionSelectionSlotUI.gameObject.SetActive(false);
        }
    }

    private void HandleCompanionSelectionSlotClicked(string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            return;
        }

        try
        {
            _selectedCompanionSelectionId = null;

            if (TryRemoveCompanion(companionId))
            {
                return;
            }

            if (TrySetCompanionToSelectedPosition(companionId))
            {
                return;
            }

            TrySetCompanionToEmptyPosition(companionId);
        }
        finally
        {
            ClearSelectedCompanionPosition();
        }
    }

    private bool TrySetCompanionToSelectedPosition(string companionId)
    {
        if (_selectedCompanionPosition == INVALID_COMPANION_POSITION)
        {
            return false;
        }

        bool isCompanionSet = BattleManager.Instance.RequestSetCompanionToPosition(_selectedCompanionPosition, companionId);
        return isCompanionSet;
    }

    private bool TrySetCompanionToEmptyPosition(string companionId)
    {
        bool isCompanionSet = BattleManager.Instance.RequestSetCompanionToEmptyPosition(companionId);

        if (!isCompanionSet)
        {
            _selectedCompanionSelectionId = companionId;
        }

        return isCompanionSet;
    }

    private bool TryRemoveCompanion(string companionId)
    {
        bool isCompanionRemoved = BattleManager.Instance.RequestRemoveCompanion(companionId);
        return isCompanionRemoved;
    }

    private void HandleStartBattleButtonClicked()
    {
        gameObject.SetActive(false);
    }
}