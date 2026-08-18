using System;
using System.Collections.Generic;

public class BattleFormationViewModel : ViewModelBase<BattleFormationModel>
{
    public event Action<int> CompanionSlotChanged;

    public string MainId
    {
        get { return _model.MainId; }
    }

    public IReadOnlyList<string> CompanionIds
    {
        get { return _model.CompanionIds; }
    }

    public IReadOnlyList<string> EnemyIds
    {
        get { return _model.EnemyIds; }
    }

    public BattleFormationViewModel(BattleFormationModel model) : base(model)
    {
        _model.CompanionSlotChanged += OnCompanionSlotChanged;
    }
        
    //TODO: 이게 필요한가에 대한 고찰
    public override void UnBind()
    {
        _model.CompanionSlotChanged -= OnCompanionSlotChanged;

        base.UnBind();
    }

    public void RequestInitializeForStage(string stageId)
    {
        _model.InitializeForStage(stageId);
    }

    public int FindCompanionSlotIndex(string companionId)
    {
        int companionSlotIndex = _model.FindCompanionSlotIndex(companionId);
        return companionSlotIndex;
    }

    public bool RequestAddCompanion(string companionId)
    {
        bool isCompanionAdded = _model.TryAddCompanionToEmptySlot(companionId);
        return isCompanionAdded;
    }

    public bool RequestSetCompanion(int slotIndex, string companionId)
    {
        bool isCompanionSet = _model.TrySetCompanion(slotIndex, companionId);
        return isCompanionSet;
    }

    public bool RequestSwapCompanion(int firstSlotIndex, int secondSlotIndex)
    {
        bool isCompanionSwapped = _model.TrySwapCompanion(firstSlotIndex, secondSlotIndex);
        return isCompanionSwapped;
    }

    public bool RequestRemoveCompanion(int slotIndex)
    {
        bool isCompanionRemoved = _model.TryRemoveCompanion(slotIndex);
        return isCompanionRemoved;
    }

    public void RequestBattleStart()
    {
        BattleManager.Instance.StartBattle(MainId, CompanionIds, EnemyIds);
    }

    private void OnCompanionSlotChanged(int index)
    {
        CompanionSlotChanged?.Invoke(index);
    }
}