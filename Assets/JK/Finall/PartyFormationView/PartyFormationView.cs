using System.Collections.Generic;
using UnityEngine;

public class PartyFormationView : ViewBase<BattleFormationViewModel>
{
    private const int INVALID_COMPANION_FORMATION_SLOT_INDEX = -1;

    [Header("Player Formation")]
    [SerializeField] private FormationSlotUI _mainFormationSlot;
    [SerializeField] private CompanionFormationSlotUI[] _companionFormationSlots = new CompanionFormationSlotUI[BattleConstants.MAX_COMPANION_COUNT];

    [Header("Enemy Formation")]
    [SerializeField] private FormationSlotUI[] _enemyFormationSlots = new FormationSlotUI[BattleConstants.MAX_ENEMY_COUNT];

    [Header("Companion Selection")]
    [SerializeField] private Transform _companionSelectionContent;
    [SerializeField] private CompanionSelectionSlotUI _companionSelectionSlotPrefab;

    [Header("Start Battle Button")]
    [SerializeField] private UIButtonComponent StartBattle;

    private int _selectedCompanionFormationSlotIndex = INVALID_COMPANION_FORMATION_SLOT_INDEX;
    private string _selectedCompanionId = null;

    private readonly List<CompanionSelectionSlotUI> _companionSelectionSlotViews = new List<CompanionSelectionSlotUI>();

    private void Awake()
    {
        ValidateReference();

        InitializeCompanionFormationSlots();
        StartBattle.BindButtonEvent(HandleStartButtonClicked);//Test
    }

    private void OnEnable()
    {
        BindViewModel(GameManager.ViewModel.PartyFormationViewModel); //추후 Awake에 하기로

        Subscribe();

        SetStage(""); //Test
        CreateCompanionSelectionSlots(); //Test

        _viewModel.InitializeModel(); //Test
    }

    protected void OnDisable()
    {
        UnSubscribe();
    }

    private void OnDestroy()
    {
        CleanupCompanionFormationSlots();
        StartBattle.UnBindButtonAllEvent(); //Test
    }
    public void SetStage(string stageId)
    {
        _viewModel.RequestInitializeForStage(stageId);
    }

    protected override void Subscribe()
    {
        base.Subscribe();
        _viewModel.CompanionSlotChanged += HandleCompanionSlotChanged;
    }

    protected override void UnSubscribe()
    {
        base.UnSubscribe();
        _viewModel.CompanionSlotChanged -= HandleCompanionSlotChanged;
    }

    //TODO UI용 뷰모델이 생기면 나중에 변경
    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_viewModel.MainId):
                RefreshMainFormationSlot();
                break;
            case nameof(_viewModel.CompanionIds):
                RefreshCompanionFormationSlots();
                break;
            case nameof(_viewModel.EnemyIds):
                RefreshEnemyFormationSlots();
                break;
        }
    }

    private void ValidateReference()
    {
        UnityUtility.ValidateReference(_mainFormationSlot, nameof(_mainFormationSlot));
        UnityUtility.ValidateArrayReference(_companionFormationSlots, nameof(_companionFormationSlots));
        UnityUtility.ValidateArrayReference(_enemyFormationSlots, nameof(_enemyFormationSlots));
        UnityUtility.ValidateReference(_companionSelectionContent, nameof(_companionSelectionContent));
        UnityUtility.ValidateReference(_companionSelectionSlotPrefab, nameof(_companionSelectionSlotPrefab));
        UnityUtility.ValidateReference(StartBattle, nameof(StartBattle));
    }

    private void HandleCompanionSlotChanged(int index)
    {
        RefreshCompanionFormationSlot(index);
    }

    private void HandleStartButtonClicked()
    {
     //   BattleManager.Instance.StartBattle("Stage_01");
    }

    #region 주인공 슬롯 관련 로직

    //TODO 플레이어 모델 추가시 연동 작업
    private void RefreshMainFormationSlot()
    {
        string mainCharacterId = _viewModel.MainId;

        Dictionary<string, CompanionData> companionDataTable = GameManager.DataTable.CompanionDataTable;

        if (!companionDataTable.TryGetValue(mainCharacterId, out CompanionData companionData))
        {
            throw new KeyNotFoundException($"메인 캐릭터 ID '{mainCharacterId}' 를 찾을 수 없습니다.");
        }

        _mainFormationSlot.SetSprite(companionData.FormationSpriteKey);
    }

    #endregion

    #region 동료 슬롯 관련 로직

    private void RefreshCompanionFormationSlots()
    {
        for (int index = 0; index < _companionFormationSlots.Length; index++)
        {
            RefreshCompanionFormationSlot(index);
        }
    }

    private void RefreshCompanionFormationSlot(int index)
    {
        string companionDataId = _viewModel.CompanionIds[index];

        if (string.IsNullOrWhiteSpace(companionDataId))
        {
            _companionFormationSlots[index].SetSprite(null);
            return;
        }

        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionDataId);

        if (companionData == null)
        {
            Debug.LogError($"ID '{companionDataId}'에 해당하는 CompanionData를 찾을 수 없습니다.");
            return;
        }

        _companionFormationSlots[index].SetSprite(companionData.FormationSpriteKey);
    }

    private void InitializeCompanionFormationSlots()
    {
        for (int slotIndex = 0; slotIndex < _companionFormationSlots.Length; slotIndex++)
        {
            CompanionFormationSlotUI companionFormationSlotView = _companionFormationSlots[slotIndex];

            companionFormationSlotView.SetSlotIndex(slotIndex);
            companionFormationSlotView.SlotClicked += HandleCompanionFormationSlotClicked;
        }
    }

    private void CleanupCompanionFormationSlots()
    {
        foreach (CompanionFormationSlotUI companionFormationSlotView in _companionFormationSlots)
        {
            companionFormationSlotView.ClearSlotIndex();
            companionFormationSlotView.SlotClicked -= HandleCompanionFormationSlotClicked;
        }
    }

    private void RefreshCompanionSlotSelection()
    {
        foreach (CompanionFormationSlotUI companionFormationSlot in _companionFormationSlots)
        {
            bool isSelected = _selectedCompanionFormationSlotIndex == companionFormationSlot.SlotIndex;
            companionFormationSlot.SetSelected(isSelected);
        }
    }

    private void HandleCompanionFormationSlotClicked(int slotIndex)
    {
        if (TrySetCompanionToSelectedSlot(slotIndex))
        {
            return;
        }

        if (TrySelectCompanionFormationSlot(slotIndex))
        {
            return;
        }

        try
        {
            if (TrySwapCompanionFormationSlot(slotIndex))
            {
                return;
            }

            TryRemoveCompanionFromFormationSlot(slotIndex);
        }
        finally
        {
            ClearCompanionFormationSlotSelection();
        }
    }

    private bool TrySetCompanionToSelectedSlot(int slotIndex)
    {
        if (_selectedCompanionId == null)
        {
            return false;
        }

        bool isSet = _viewModel.RequestSetCompanion(slotIndex, _selectedCompanionId);

        if (!isSet)
        {
            Debug.LogError($"'슬롯 인덱스: {slotIndex}, 동료 아이디: {_selectedCompanionId}' 동료 편성 배치 요청이 실패했습니다.");
            return false;
        }

        _selectedCompanionId = null;
        return true;
    }

    private bool TrySelectCompanionFormationSlot(int slotIndex)
    {
        if (_selectedCompanionFormationSlotIndex >= 0)
        {
            return false;
        }

        SetCompanionFormationSlotSelection(slotIndex);

        return true;
    }

    private bool TrySwapCompanionFormationSlot(int slotIndex)
    {
        if (_selectedCompanionFormationSlotIndex == slotIndex)
        {
            return false;
        }

        bool isSwapped = _viewModel.RequestSwapCompanion(slotIndex, _selectedCompanionFormationSlotIndex);

        if (!isSwapped)
        {
            Debug.LogError($"동료 편성 위치 교체 요청이 실패했습니다.");
            return false;
        }

        return true;
    }

    private bool TryRemoveCompanionFromFormationSlot(int slotIndex)
    {
        string characterId = _viewModel.CompanionIds[slotIndex];

        if (string.IsNullOrWhiteSpace(characterId))
        {
            return false;
        }

        bool isRemoved = _viewModel.RequestRemoveCompanion(slotIndex);

        if (!isRemoved)
        {
            Debug.LogError($"'슬롯 인덱스: {slotIndex}, 동료 아이디: {characterId}' 동료 편성 해제 요청이 실패했습니다.");
            return false;
        }

        return true;
    }

    private void SetCompanionFormationSlotSelection(int slotIndex)
    {
        _selectedCompanionFormationSlotIndex = slotIndex;

        RefreshCompanionSlotSelection();
    }

    private void ClearCompanionFormationSlotSelection()
    {
        _selectedCompanionFormationSlotIndex = INVALID_COMPANION_FORMATION_SLOT_INDEX;

        RefreshCompanionSlotSelection();
    }

    #endregion

    #region 적 슬롯 관련 로직

    private void RefreshEnemyFormationSlots()
    {
        for (int index = 0; index < _enemyFormationSlots.Length; index++)
        {
            RefreshEnemyFormationSlot(index);
        }
    }

    private void RefreshEnemyFormationSlot(int index)
    {
        string enemyDataId = _viewModel.EnemyIds[index];

        if (string.IsNullOrWhiteSpace(enemyDataId))
        {
            _enemyFormationSlots[index].SetSprite(null);
            return;
        }

        EnemyData enemyData = GameManager.DataTable.GetEnemyData(enemyDataId);

        if (enemyData == null)
        {
            Debug.LogError($"ID '{enemyDataId}'에 해당하는 EnemyData를 찾을 수 없습니다.");
            return;
        }

        _enemyFormationSlots[index].SetSprite(enemyData.FormationSpriteKey);
    }

    #endregion 

    #region 동료 선택 관련 로직

    //TODO 보유 캐릭터 모델 추가시 연동 작업 / 풀로 슬롯 관리하기
    private void CreateCompanionSelectionSlots()
    {
        Dictionary<string, CompanionData> companionDataTable = GameManager.DataTable.CompanionDataTable;

        foreach (CompanionData companionData in companionDataTable.Values)
        {
            if (companionData == null)
            {
                continue;
            }

            CompanionSelectionSlotUI companionSelectionSlotView = Instantiate(_companionSelectionSlotPrefab, _companionSelectionContent);

            companionSelectionSlotView.Initialize(companionData.Id);
            companionSelectionSlotView.SetSprite(companionData.FormationSpriteKey);

            companionSelectionSlotView.SlotClicked += HandleCompanionSelectionSlotClicked;

            _companionSelectionSlotViews.Add(companionSelectionSlotView);
        }
    }

    private void HandleCompanionSelectionSlotClicked(string companionDataId)
    {
        if (string.IsNullOrWhiteSpace(companionDataId))
        {
            return;
        }

        try
        {
            _selectedCompanionId = null;
      
            if (TryRemoveCompanionFromFormationSlot(companionDataId))
            {
                return;
            }

            if (TrySetCompanionToSelectedSlot(companionDataId))
            {
                return;
            }

            TrySetCompanionToEmptySlot(companionDataId);
        }
        finally
        {
            ClearCompanionFormationSlotSelection();
        }
    }

    private bool TryRemoveCompanionFromFormationSlot(string companionDataId)
    {
        int existingSlotIndex = _viewModel.FindCompanionSlotIndex(companionDataId);

        if (existingSlotIndex < 0)
        {
            return false;
        }

        bool isRemoved = _viewModel.RequestRemoveCompanion(existingSlotIndex);

        if (!isRemoved)
        {
            Debug.LogError($"'슬롯 인덱스: {existingSlotIndex}, 동료 아이디: {companionDataId}' 동료 편성 해제 요청이 실패했습니다.");
            return false;
        }

        return true;
    }

    private bool TrySetCompanionToSelectedSlot(string companionDataId)
    {
        if (_selectedCompanionFormationSlotIndex < 0)
        {
            return false;
        }

        bool isSet = _viewModel.RequestSetCompanion(_selectedCompanionFormationSlotIndex, companionDataId);

        if (!isSet)
        {
            Debug.LogError($"'슬롯 인덱스: {_selectedCompanionFormationSlotIndex}, 동료 아이디: {companionDataId}' 동료 편성 배치 요청이 실패했습니다.");
            return false;
        }

        return true;
    }

    private bool TrySetCompanionToEmptySlot(string companionDataId)
    {
        bool isAdded = _viewModel.RequestAddCompanion(companionDataId);

        if (!isAdded)
        {
            _selectedCompanionId = companionDataId;
            return false;
        }

        return true;
    }
 
    #endregion
}