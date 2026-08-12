using System.Collections.Generic;
using UnityEngine;

public class PartyFormationView : ViewBase<PartyFormationViewModel>
{
    [Header("Leader")]
    [SerializeField] private PartySlotView _mainSlot;

    [Header("Supports")]
    [SerializeField] private PartySlotView[] _supportSlots = new PartySlotView[GameSettings.MaxSupportCount];

    [Header("Character List")]
    [SerializeField] private Transform _characterContent;
    [SerializeField] private CharacterSlotView _characterSlotPrefab;

    private readonly List<CharacterSlotView> _characterSlots = new List<CharacterSlotView>();

    private int _selectedSupportSlot = -1;
    private ColorData _selectedCharacterSlot = null;

    #region Test

    private readonly Dictionary<string, ColorData> _colorDictionary = new Dictionary<string, ColorData>();

    private void TestMethod()
    {
        _colorDictionary.Add("Red", new ColorData("Red", Color.red));
        _colorDictionary.Add("Green", new ColorData("Green", Color.green));
        _colorDictionary.Add("Blue", new ColorData("Blue", Color.blue));
        _colorDictionary.Add("Yellow", new ColorData("Yellow", Color.yellow));

        PartyFormationModel partyFormationModel = new PartyFormationModel();
        PartyFormationViewModel partyFormationViewModel = new PartyFormationViewModel(partyFormationModel);
        BindViewModel(partyFormationViewModel);
    }

    #endregion

    private void Awake()
    {
        InitializeSupportSlots();
        TestMethod();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _viewModel.SupportCharacterIdChanged -= OnSupportCharacterIdChanged;
        _viewModel = null;
    }

    protected override void OnBindViewModel()
    {
        _viewModel.SupportCharacterIdChanged += OnSupportCharacterIdChanged;
        RefreshLeaderSlot();
        InitializeCharacterList();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_viewModel.SupportCharacterIds):
                RefreshSupportPartySlots();
                break;
        }
    }

    private void InitializeSupportSlots()
    {
        for (int slotIndex = 0; slotIndex < _supportSlots.Length; slotIndex++)
        {
            _supportSlots[slotIndex].Initialize(slotIndex, OnSupportSlotClicked);
        }
    }

    private void InitializeCharacterList()
    {
        foreach (ColorData colorData in _colorDictionary.Values)
        {
            if (colorData.Color == Color.red)
            {
                continue;
            }

            CharacterSlotView slot = Instantiate(_characterSlotPrefab, _characterContent);
            slot.Initialize(colorData, OnCharacterClicked);
            _characterSlots.Add(slot);
        }
    }

    private void RefreshLeaderSlot()
    {
        string mainCharacterId = _viewModel.MainCharacterId;

        if (!_colorDictionary.TryGetValue(mainCharacterId, out ColorData colorData))
        {
            throw new KeyNotFoundException($"메인 캐릭터 ID '{mainCharacterId}' 를 찾을 수 없습니다.");
        }

        _mainSlot.SetCharacter(colorData);
    }

    private void RefreshSupportPartySlots()
    {
        for (int index = 0; index < _supportSlots.Length; index++)
        {
            OnSupportCharacterIdChanged(index);
        }
    }

    private void OnSupportCharacterIdChanged(int index)
    {
        string supportyCharacterId = _viewModel.SupportCharacterIds[index];

        if (string.IsNullOrWhiteSpace(supportyCharacterId))
        {
            _supportSlots[index].SetCharacter(null);
            return;
        }

        if (!_colorDictionary.TryGetValue(supportyCharacterId, out ColorData colorData))
        {
            throw new KeyNotFoundException($"메인 캐릭터 ID '{supportyCharacterId}' 를 찾을 수 없습니다.");
        }

        _supportSlots[index].SetCharacter(colorData);
    }

    private void RefreshSupportSlotSelection()
    {
        for (int slotIndex = 0; slotIndex < _supportSlots.Length; slotIndex++)
        {
            _supportSlots[slotIndex].SetSelected(slotIndex == _selectedSupportSlot);
        }
    }

    private void ClearSupportSlotSelection()
    {
        _selectedSupportSlot = -1;

        RefreshSupportSlotSelection();
    }

    private void OnSupportSlotClicked(int slotIndex)
    {
        if (_selectedCharacterSlot != null)
        {
            bool isSet = _viewModel.RequestSetSupport(slotIndex, _selectedCharacterSlot.Id);

            if (isSet)
            {
                _selectedCharacterSlot = null;
            }

            return;
        }

        if (_selectedSupportSlot < 0)
        {
            _selectedSupportSlot = slotIndex;

            RefreshSupportSlotSelection();

            return;
        }

        if (_selectedSupportSlot != slotIndex)
        {
            bool isSwaped = _viewModel.RequestSwapSupport(slotIndex, _selectedSupportSlot);

            if (isSwaped)
            {
                ClearSupportSlotSelection();
            }

            return;
        }

        if (_supportSlots[slotIndex].HasCharacter())
        {
            bool isRemoved = _viewModel.RequestRemoveSupport(slotIndex);

            if (isRemoved)
            {
                ClearSupportSlotSelection();
            }
        }
    }

    private void OnCharacterClicked(ColorData colorData)
    {
        if (colorData == null)
        {
            return;
        }

        if (_selectedCharacterSlot != null)
        {
            _selectedCharacterSlot = null;
        }

        int existingSlotIndex = _viewModel.RequestFindSupportSlotIndex(colorData.Id);

        if (existingSlotIndex >= 0)
        {
            bool isRemoved = _viewModel.RequestRemoveSupport(existingSlotIndex);
            
            if (isRemoved)
            {
                ClearSupportSlotSelection();
            }

            return;
        }

        if (_selectedSupportSlot >= 0)
        {
            bool isSupportSet = _viewModel.RequestSetSupport(_selectedSupportSlot, colorData.Id);

            if (isSupportSet)
            {
                ClearSupportSlotSelection();
            }

            return;
        }

        bool isAdded = _viewModel.RequestAddSupport(colorData.Id);

        if (!isAdded)
        {
            _selectedCharacterSlot = colorData;
            return;
        }

        ClearSupportSlotSelection();
    }
}