using System.Collections.Generic;
using UnityEngine;

public class CompanionInventoryView : ViewBase
{
    [SerializeField] private Transform _slotRoot;
    [SerializeField] private TMPro.TMP_Dropdown _sortDropdown;

    private Dictionary<string, CompanionInventorySlotView> _slotDict = new();

    private CompanionInventoryViewModel _inventoryViewModel;


    private void OnEnable()
    {
        if (_inventoryViewModel == null)
        {
            BindViewModel();
        }

        Subscribe();
        _sortDropdown.onValueChanged.AddListener(OnClickSort);
    }

    private void OnDisable()
    {
        UnSubscribe();
        _sortDropdown.onValueChanged.RemoveListener(OnClickSort);
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_inventoryViewModel != null)
        {
            _inventoryViewModel.UnBind();
            _inventoryViewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _inventoryViewModel = GameManager.ViewModel.CreateCompanionInventoryViewModel();
    }

    protected override void Subscribe()
    {
        _inventoryViewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _inventoryViewModel.OnContainerChanged_ViewModel += OnContainerChanged;
        _inventoryViewModel.InitializeModel();
    }

    protected override void UnSubscribe()
    {
        if (_inventoryViewModel != null)
        {
            _inventoryViewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
            _inventoryViewModel.OnContainerChanged_ViewModel -= OnContainerChanged;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(CompanionModel.Companions):
                ResetSlotAndCreateAll();
                break;
        }
    }
    private void OnClickSort(int index)
    {
        _inventoryViewModel.SetSort(index);

        SortAllSlot();
    }

    private void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, CompanionState companionState)
    {
        if (propertyName == nameof(CompanionModel.Companions) == false)
        {
            return;
        }

        switch (changedEvent)
        {
            case ContainerPropertyChangedEvent.Add:
                {
                    CreateSlot(companionState);
                    SortOneSlot(companionState);
                }
                break;

            case ContainerPropertyChangedEvent.Remove:
                {
                    if (_slotDict.ContainsKey(companionState.CompanionId))
                    {
                        var slotView = _slotDict[companionState.CompanionId];
                        Destroy(slotView.gameObject);
                        _slotDict.Remove(companionState.CompanionId);
                    }
                }
                break;

            case ContainerPropertyChangedEvent.Update:
                {
                    SortOneSlot(companionState);
                }
                break;
        }
    }

    private void SortOneSlot(CompanionState companionState)
    {
        CompanionInventorySlotView slot = _slotDict[companionState.CompanionId];

        slot.transform.SetSiblingIndex(_inventoryViewModel.GetItemIndex(companionState));
    }

    private void SortAllSlot()
    {
        for (int i = 0; i < _inventoryViewModel.Items.Count; i++)
        {
            CompanionState companionState = _inventoryViewModel.Items[i];
            _slotDict[companionState.CompanionId].transform.SetSiblingIndex(i);
        }
    }

    private void ResetSlotAndCreateAll()
    {
        RemoveAllSlot();

        foreach (var companionState in _inventoryViewModel.Items)
        {
            CreateSlot(companionState);
        }
    }


    private void RemoveAllSlot()
    {
        if (_slotDict.Count > 0)
        {
            foreach (var slot in _slotDict)
            {
                Destroy(slot.Value.gameObject);
            }
            _slotDict.Clear();
        }
    }

    private void CreateSlot(CompanionState companionState)
    {
        var slotPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(typeof(CompanionInventorySlotView)));
        var slotInstance = Instantiate(slotPrefab, _slotRoot);
        if (slotInstance == null)
            return;

        var slotComponent = slotInstance.GetComponent<CompanionInventorySlotView>();
        if (slotComponent == null)
            return;

        slotComponent.Init(companionState.CompanionId, OnClickSlot);
        slotComponent.PlayOpenAnimation();

        _slotDict.Add(companionState.CompanionId, slotComponent);
    }

    private void OnClickSlot(string companionId)
    {
        GameManager.UI.OpenCompanionDetailPopup(_inventoryViewModel.GetCompanionState(companionId), () => OnLevelUp(companionId));
    }

    private LevelUpResult OnLevelUp(string companionId)
    {
        return _inventoryViewModel.TempLevelUp(companionId);
    }
}
