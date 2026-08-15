using System;
using System.Collections.Generic;

public class CompanionModel
{
    private List<CompanionState> _companions = new();
    private Dictionary<string, CompanionState> _companionById = new();

    public IReadOnlyList<CompanionState> Companions => _companions;

    public event Action<string> CompanionChanged;

    public CompanionModel(IEnumerable<CompanionState> saveData)
    {
        foreach (CompanionState companionSaveData in saveData)
        {
            CompanionState companion = new CompanionState(companionSaveData.CompanionId, companionSaveData.Level);

            _companions.Add(companion);
            _companionById.Add(companion.CompanionId, companion);
        }
    }

    public CompanionState GetCompanion(string companionId)
    {
        if (string.IsNullOrEmpty(companionId))
        {
            return null;
        }

        return _companionById.TryGetValue(companionId, out CompanionState companion) ? companion : null;
    }

    public bool AddCompanion(string companionId)
    {
        if (_companionById.ContainsKey(companionId))
        {
            return false;
        }

        CompanionState companion = new CompanionState(companionId, 1);

        _companions.Add(companion);
        _companionById.Add(companionId, companion);
        CompanionChanged?.Invoke(companionId);

        return true;
    }

    public void LevelUp(string companionId)
    {
        if (_companionById.TryGetValue(companionId, out CompanionState companion))
        {
            companion.LevelUp();
            CompanionChanged?.Invoke(companionId);
        }
    }
}
