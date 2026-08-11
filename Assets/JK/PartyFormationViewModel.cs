using System;
using System.Collections.Generic;

public class PartyFormationViewModel : ViewModelBase<PartyFormationModel>
{
    public event Action<int> SupportCharacterIdChanged;

    public string MainCharacterId
    {
        get { return _model.MainCharacterId; }
    }

    public IReadOnlyList<string> SupportCharacterIds
    {
        get { return _model.SupportCharacterIds; }
    }

    public PartyFormationViewModel(PartyFormationModel model) : base(model)
    {
        _model.SupportCharacterIdChanged += OnSupportCharacterIdChanged;
    }

    public override void UnBind()
    {
        _model.SupportCharacterIdChanged -= OnSupportCharacterIdChanged;

        base.UnBind();
    }

    public int RequestFindSupportSlotIndex(string characterId)
    {
        int existingSlotIndex = _model.FindSupportSlotIndex(characterId);
        return existingSlotIndex;
    }

    public bool RequestAddSupport(string characterId)
    {
        bool isSupportAdd = _model.TryAddSupport(characterId);
        return isSupportAdd;
    }

    public bool RequestSetSupport(int slotIndex, string characterId)
    {
        bool isSupportSet = _model.TrySetSupport(slotIndex, characterId);
        return isSupportSet;
    }

    public bool RequestSwapSupport(int firstSlotIndex, int secondSlotIndex)
    {
        bool isSwapped = _model.SwapSupport(firstSlotIndex, secondSlotIndex);
        return isSwapped;
    }

    private void OnSupportCharacterIdChanged(int index)
    {
        SupportCharacterIdChanged?.Invoke(index);
    }
}
