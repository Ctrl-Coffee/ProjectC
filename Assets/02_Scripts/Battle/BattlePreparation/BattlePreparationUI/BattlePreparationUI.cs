using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattlePreparationUI : UIBase
{
    [Header("Total Combat Power")]
    [SerializeField] private TMP_Text _playerPowerText;
    [SerializeField] private TMP_Text _enemyPowerText;

    [Header("StageName")]
    [SerializeField] private TMP_Text _stageNameText;

    [Header("Companion Selection")]
    [SerializeField] private Transform _companionSelectionContent;
    [SerializeField] private CompanionSelectionSlotUI _companionSelectionSlotPrefab;

    [Header("Start Battle")]
    [SerializeField] private UIButtonComponent _startBattleButton;

    [Header("Back Battle")]
    [SerializeField] private UIButtonComponent _backBattleButton;

    private Camera _mainCamera;

    private int _selectedCompanionBattlePosition = Const.INVALID_BATTLE_POSITION;
    private string _selectedCompanionSelectionId;

    private readonly List<CompanionSelectionSlotUI> _companionSelectionSlotPool = new List<CompanionSelectionSlotUI>();

    private void Awake()
    {
        ValidateReference();

        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        UpdateStartButton();

        RefreshStageNameText();
        RefreshCombatPower();
        RefreshCompanionSelectionSlots();

        _startBattleButton.BindButtonEvent(HandleStartBattleButtonClicked);
        _backBattleButton.BindButtonEvent(HandleBackBattleButtonClicked);

        GameManager.Battle.CompanionChanged += HandleCompanionChanged;
    }

    private void Update()
    {
        Pointer pointer = Pointer.current;

        if (pointer == null || !pointer.press.wasPressedThisFrame)
        {
            return;
        }

        if (!TryGetClickedCompanionPosition(out int battlePosition))
        {
            return;
        }

        HandleCompanionPositionClicked(battlePosition);
    }

    private void OnDisable()
    {
        _startBattleButton.UnBindButtonAllEvent();
        _backBattleButton.UnBindButtonAllEvent();

        ClearSelectedCompanionPosition();
        ClearSelectedCompanionId();

        GameManager.Battle.CompanionChanged -= HandleCompanionChanged;
    }

    private void OnDestroy()
    {
        foreach (CompanionSelectionSlotUI companionSelectionSlot in _companionSelectionSlotPool)
        {
            if (companionSelectionSlot == null)
            {
                continue;
            }

            companionSelectionSlot.SlotClicked -= HandleCompanionSelectionSlotClicked;
        }
    }

    private void ValidateReference()
    {
        UnityUtility.ValidateReference(_playerPowerText, nameof(_playerPowerText));
        UnityUtility.ValidateReference(_enemyPowerText, nameof(_enemyPowerText));
        UnityUtility.ValidateReference(_stageNameText, nameof(_stageNameText));
        UnityUtility.ValidateReference(_companionSelectionContent, nameof(_companionSelectionContent));
        UnityUtility.ValidateReference(_companionSelectionSlotPrefab, nameof(_companionSelectionSlotPrefab));
        UnityUtility.ValidateReference(_startBattleButton, nameof(_startBattleButton));
        UnityUtility.ValidateReference(_backBattleButton, nameof(_backBattleButton));
    }

    private bool TryGetClickedCompanionPosition(out int battlePosition)
    {
        battlePosition = Const.INVALID_BATTLE_POSITION;

        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Vector2 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

        Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

        if (hitCollider == null)
        {
            return false;
        }

        if (!hitCollider.TryGetComponent(out CompanionBattleUnitView companionBattleUnitView))
        {
            return false;
        }

        battlePosition = companionBattleUnitView.BattlePosition;
        return true;
    }

    private void HandleCompanionPositionClicked(int battlePosition)
    {
        if (TrySetCompanionToSelectedPosition(battlePosition))
        {
            return;
        }

        if (TrySelectCompanionPosition(battlePosition))
        {
            return;
        }

        try
        {
            if (TrySwapCompanion(battlePosition))
            {
                return;
            }

            TryRemoveCompanion(battlePosition);
        }
        finally
        {
            ClearSelectedCompanionPosition();
        }
    }

    private bool TrySetCompanionToSelectedPosition(int battlePosition)
    {
        if (string.IsNullOrWhiteSpace(_selectedCompanionSelectionId))
        {
            return false;
        }

        bool isCompanionSet = GameManager.Battle.RequestSetCompanionToPosition(battlePosition, _selectedCompanionSelectionId);

        if (isCompanionSet)
        {
            _selectedCompanionSelectionId = null;
        }

        return isCompanionSet;
    }

    private bool TrySelectCompanionPosition(int battlePosition)
    {
        if (_selectedCompanionBattlePosition != Const.INVALID_BATTLE_POSITION)
        {
            return false;
        }

        _selectedCompanionBattlePosition = battlePosition;

        return true;
    }

    private bool TrySwapCompanion(int battlePosition)
    {
        if (_selectedCompanionBattlePosition == battlePosition)
        {
            return false;
        }

        bool isCompanionSwapped = GameManager.Battle.RequestSwapCompanion(_selectedCompanionBattlePosition, battlePosition);
        return isCompanionSwapped;
    }

    private bool TryRemoveCompanion(int battlePosition)
    {
        bool isCompanionRemoved = GameManager.Battle.RequestRemoveCompanion(battlePosition);
        return isCompanionRemoved;
    }

    private void ClearSelectedCompanionPosition()
    {
        _selectedCompanionBattlePosition = Const.INVALID_BATTLE_POSITION;
    }

    private void ClearSelectedCompanionId()
    {
        _selectedCompanionSelectionId = null;
    }

    private void RefreshCompanionSelectionSlots()
    {
        Dictionary<string, CompanionState> companionDictionary = GameManager.Session.Companion.Companions;

        int activeSlotIndex = 0;

        foreach (CompanionState companionState in companionDictionary.Values)
        {
            if (companionState == null)
            {
                continue;
            }

            CompanionSelectionSlotUI companionSelectionSlotUI = GetCompanionSelectionSlot(activeSlotIndex);

            companionSelectionSlotUI.Initialize(companionState.CompanionId);
            companionSelectionSlotUI.gameObject.SetActive(true);

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
            companionSelectionSlotUI.Clear();
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
        if (_selectedCompanionBattlePosition == Const.INVALID_BATTLE_POSITION)
        {
            return false;
        }

        bool isCompanionSet = GameManager.Battle.RequestSetCompanionToPosition(_selectedCompanionBattlePosition, companionId);
        return isCompanionSet;
    }

    private bool TrySetCompanionToEmptyPosition(string companionId)
    {
        bool isCompanionSet = GameManager.Battle.RequestSetCompanionToEmptyPosition(companionId);

        if (!isCompanionSet)
        {
            _selectedCompanionSelectionId = companionId;
        }

        return isCompanionSet;
    }

    private bool TryRemoveCompanion(string companionId)
    {
        bool isCompanionRemoved = GameManager.Battle.RequestRemoveCompanion(companionId);
        return isCompanionRemoved;
    }

    private void UpdatePlayerCombatPower()
    {
        int playerTotalCombatPower = GameManager.Battle.GetPlayerTotalCombatPower();
        _playerPowerText.text = playerTotalCombatPower.ToString();
    }

    private void UpdateEnemyCombatPower()
    {
        int enemyTotalCombatPower = GameManager.Battle.GetEnemyTotalCombatPower();
        _enemyPowerText.text = enemyTotalCombatPower.ToString();
    }

    private void RefreshStageNameText()
    {
        _stageNameText.text = GameManager.Stage.StageName;
    }

    private void RefreshCombatPower()
    {
        UpdatePlayerCombatPower();
        UpdateEnemyCombatPower();
    }

    private void HandleStartBattleButtonClicked()
    {
        GameManager.Battle.StartBattle();
        GameManager.UI.CloseBattlePreparationUI();
    }

    private void HandleBackBattleButtonClicked()
    {
        GameManager.Battle.ExitBattle();
        GameManager.UI.CloseBattlePreparationUI();
    }

    private void HandleCompanionChanged()
    {
        UpdatePlayerCombatPower();
    }

    private void UpdateStartButton()
    {
        long dreamPoint = GameManager.Session.Currency.DreamPoint;
        long dreamPointCost = GameManager.Stage.DpCost;

        _startBattleButton.SetInteractable(dreamPoint >= dreamPointCost);
    }
}