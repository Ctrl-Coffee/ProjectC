using System;
using System.Collections.Generic;

public class CompanionModel : ModelBase, ContainerPropertyChanged<CompanionState>
{
    private Dictionary<string, CompanionState> _companions = new();

    public event Action<string, ContainerPropertyChangedEvent, CompanionState> ContainerPropertyChanged;

    public Dictionary<string, CompanionState> Companions { get => _companions; }

    public CompanionModel(List<CompanionDto> companions)
    {
        foreach (CompanionDto companionDto in companions)
        {
            CompanionState companState = new CompanionState(companionDto);

            _companions.Add(companionDto.companionId, companState);
        }

        InitializeOnce();
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Companions));
    }

    public void AddCompanion(string companionId)
    {
        if (!_companions.ContainsKey(companionId))
        {
            CompanionState companion = new CompanionState(companionId, 1);
            _companions.Add(companionId, companion);

            ContainerPropertyChanged?.Invoke(nameof(Companions), ContainerPropertyChangedEvent.Add, companion);

            SaveUtil.RequestSaveCompanion();
        }
    }

    public CompanionState GetCompanion(string companionId)
    {
        if (string.IsNullOrEmpty(companionId))
        {
            return null;
        }

        return _companions.TryGetValue(companionId, out CompanionState companion) ? companion : null;
    }

    public LevelUpResult TryLevelUp(string companionId)
    {
        if (!_companions.TryGetValue(companionId, out CompanionState companion))
        {
            return LevelUpResult.Error;
        }

        var levelData = GameManager.DataTable.GetCompanionLevelData(companion.Level);

        if (levelData.UpgradeCost == 0)
        {
            return LevelUpResult.MaxLevel;
        }

        if (!GameManager.Session.Currency.TrySpendDreamFragment((long)levelData.UpgradeCost))
        {
            return LevelUpResult.NotEnoughCurrency;
        }

        companion.LevelUp();
        ContainerPropertyChanged?.Invoke(nameof(Companions), ContainerPropertyChangedEvent.Update, companion);

        SaveUtil.RequestSaveCompanion();

        return LevelUpResult.Success;
    }

    public void RemoveCompanion(string companionId)
    {
        if (_companions.TryGetValue(companionId, out CompanionState companion))
        {
            _companions.Remove(companionId);
            ContainerPropertyChanged?.Invoke(nameof(Companions), ContainerPropertyChangedEvent.Remove, companion);
        }
    }
}
