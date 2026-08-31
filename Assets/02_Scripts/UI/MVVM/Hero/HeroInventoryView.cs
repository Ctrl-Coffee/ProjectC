using System.Collections.Generic;
using UnityEngine;

public class HeroInventoryView : ViewBase
{
    [SerializeField] private Transform _slotRoot;
    [SerializeField] private Transform _equippedSlotRoot;

    [SerializeField] private TMPro.TMP_Dropdown _sortDropdown;
    [SerializeField] private TMPro.TextMeshProUGUI _combatPowerText;

    private Dictionary<string, HeroInventorySlotView> _slotDict = new();
    private Dictionary<string, EquipmentData> _equipmentDataTable;

    private HeroInventoryViewModel _viewModel;
    private HeroInfoViewModel _heroInfoViewModel;

    private void OnEnable()
    {
        if(_equipmentDataTable == null)
        {
            LoadDataTable();
        }

        if (_viewModel == null || _heroInfoViewModel == null)
        {
            BindViewModel();
        }

        Subscribe();
        _sortDropdown.onValueChanged.AddListener(OnClickSort);
        CombatPowerRefash();
    }

    private void OnDisable()
    {
        UnSubscribe();
        _sortDropdown.onValueChanged.RemoveListener(OnClickSort);
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_viewModel != null)
        {
            _viewModel.UnBind();
            _viewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _heroInfoViewModel = GameManager.ViewModel.CreateHeroInfoViewModel();
        _viewModel = GameManager.ViewModel.CreateHeroInventoryViewModel();
        CreatEquipedSlot();
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _viewModel.OnContainerChanged_ViewModel += OnContainerChanged;
        _heroInfoViewModel.OnPropertyChanged_ViewModel += OnPropertyChangedHeroInfo;
        _viewModel.InitializeModel();
        _heroInfoViewModel.InitializeModel();
    }

    protected override void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
            _viewModel.OnContainerChanged_ViewModel -= OnContainerChanged;
            _heroInfoViewModel.OnPropertyChanged_ViewModel -= OnPropertyChangedHeroInfo;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(HeroEquipmentModel.Equipments):
                ResetSlotAndCreateAll();
                break;
        }
    }

    private void OnPropertyChangedHeroInfo(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(HeroInfoModel.CombatPower):
                CombatPowerRefash();
                break;
        }
    }

    private void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, HeroEquipmentState state)
    {
        if (propertyName == nameof(HeroEquipmentModel.Equipments) == false)
        {
            return;
        }

        switch (changedEvent)
        {
            case ContainerPropertyChangedEvent.Add:
                {
                    CreateInventorySlot(state);
                    SortOneSlot(state);
                }
                break;
            case ContainerPropertyChangedEvent.Remove:
                {
                    if (_slotDict.ContainsKey(state.HeroEquipmentId))
                    {
                        var slotView = _slotDict[state.HeroEquipmentId];
                        Destroy(slotView.gameObject);
                        _slotDict.Remove(state.HeroEquipmentId);
                    }
                }
                break;
            case ContainerPropertyChangedEvent.Update:
                {
                    SortOneSlot(state);
                }
                break;
        }
    }

    private void OnClickSort(int index)
    {
        _viewModel.SetSort(index);

        SortAllSlot();
    }

    private void SortAllSlot()
    {
        for (int i = 0; i < _viewModel.Items.Count; i++)
        {
            HeroEquipmentState state = _viewModel.Items[i];
            _slotDict[state.HeroEquipmentId].transform.SetSiblingIndex(i);
        }
    }
    private void SortOneSlot(HeroEquipmentState state)
    {
        HeroInventorySlotView slot = _slotDict[state.HeroEquipmentId];

        slot.transform.SetSiblingIndex(_viewModel.GetItemIndex(state));
    }

    private void ResetSlotAndCreateAll()
    {
        RemoveAllSlot();

        foreach (var companionState in _viewModel.Items)
        {
            CreateInventorySlot(companionState);
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

    private void CreateInventorySlot(HeroEquipmentState state)
    {
        var slotPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(typeof(HeroInventorySlotView)));
        var slotInstance = Instantiate(slotPrefab, _slotRoot);
        if (slotInstance == null)
            return;

        var slotComponent = slotInstance.GetComponent<HeroInventorySlotView>();
        if (slotComponent == null)
            return;

        EquipmentData equipmentData = _equipmentDataTable[state.HeroEquipmentId];
        EquipmentType equipmentType = Utils.ParseEnum<EquipmentType>(equipmentData.EquipmentTypeString);

        slotComponent.Init(equipmentType, state.HeroEquipmentId, OnDetailSlot, equipmentData.IconSpriteAddressableKey);
        slotComponent.PlayOpenAnimation();

        _slotDict.Add(state.HeroEquipmentId, slotComponent);
    }

    private void CreatEquipedSlot()
    {
        for(int i = 0; i < (int)EquipmentType.COUNT; i++)
        {
            var slotPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(typeof(HeroEquipmentSlotView)));
            var slotInstance = Instantiate(slotPrefab, _equippedSlotRoot);
            if (slotInstance == null)
                return;

            var slotComponent = slotInstance.GetComponent<HeroEquipmentSlotView>();
            if (slotComponent == null)
                return;

            slotComponent.Init((EquipmentType)i, OnDetailSlot);
        }
    }

    private void OnDetailSlot(string heroEquipmentId)
    {
        if(heroEquipmentId == null)
        {
            return;
        }    

        HeroEquipmentState state = _viewModel.GetHeroEquipmentState(heroEquipmentId);

        GameManager.UI.OpenEquipmentDetailPopup(() => OnLevelUp(heroEquipmentId)
        , _equipmentDataTable[heroEquipmentId], heroEquipmentId);
    }

    private LevelUpResult OnLevelUp(string heroEquipmentId)
    {
        return _viewModel.TryLevelUp(heroEquipmentId);
    }

    private void LoadDataTable()
    {
        _equipmentDataTable = GameManager.DataTable.EquipmentDataTable;
    }

    private void CombatPowerRefash()
    {
        _combatPowerText.text = Mathf.RoundToInt(_heroInfoViewModel.CombatPower).ToString("N0");
    }
}
