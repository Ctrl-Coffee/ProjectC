using System;
using System.Collections.Generic;

public class BattleFormationModel : ModelBase
{
    private readonly string _mainId;

    private readonly string[] _companionIds = new string[BattleConstants.MAX_COMPANION_COUNT];

    private readonly string[] _enemyIds = new string[BattleConstants.MAX_ENEMY_COUNT];

    public string MainId
    {
        get { return _mainId; }
    }

    public IReadOnlyList<string> CompanionIds
    {
        get { return _companionIds; }
    }

    public IReadOnlyList<string> EnemyIds
    {
        get { return _enemyIds; }
    }

    public event Action<int> CompanionSlotChanged;

    public BattleFormationModel()
    {
        //TODO 주인공 아이디 받아오기
        _mainId = "Companion_001";
    }

    public void InitializeForStage(string stageId)
    {
        string[] enemyIds = { "enemy_ch1_001", "enemy_ch1_001", "enemy_ch1_001", "enemy_ch1_001", "enemy_ch1_001", "enemy_ch1_001" }; // Test

        for (int slotIndex = 0; slotIndex < _enemyIds.Length; slotIndex++)
        {
            _enemyIds[slotIndex] = enemyIds[slotIndex];
        }
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(MainId));
        OnPropertyChanged(nameof(CompanionIds));
        OnPropertyChanged(nameof(EnemyIds));
    }

    public bool TrySetCompanion(int slotIndex, string companionId)
    {
        if (!IsValidCompanionSlotIndex(slotIndex))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        if (ContainsCompanion(companionId))
        {
            return false;
        }

        _companionIds[slotIndex] = companionId;

        OnCompanionSlotChanged(slotIndex);

        return true;
    }

    public bool TryAddCompanionToEmptySlot(string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        if (ContainsCompanion(companionId))
        {
            return false;
        }

        int emptySlotIndex = FindEmptyCompanionSlotIndex();

        if (emptySlotIndex < 0)
        {
            return false;
        }

        bool isCompanionSet = TrySetCompanion(emptySlotIndex, companionId);
        return isCompanionSet;
    }

    public bool TryRemoveCompanion(int slotIndex)
    {
        if (!IsValidCompanionSlotIndex(slotIndex))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_companionIds[slotIndex]))
        {
            return false;
        }

        _companionIds[slotIndex] = null;

        OnCompanionSlotChanged(slotIndex);

        return true;
    }

    public bool TrySwapCompanion(int firstSlotIndex, int secondSlotIndex)
    {
        if (firstSlotIndex == secondSlotIndex)
        {
            return false;
        }

        if (!IsValidCompanionSlotIndex(firstSlotIndex) || !IsValidCompanionSlotIndex(secondSlotIndex))
        {
            return false;
        }

        string firstCompanionId = _companionIds[firstSlotIndex];
        string secondCompanionId = _companionIds[secondSlotIndex];

        _companionIds[firstSlotIndex] = secondCompanionId;
        _companionIds[secondSlotIndex] = firstCompanionId;

        OnPropertyChanged(nameof(CompanionIds));

        return true;
    }

    public int FindCompanionSlotIndex(string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            return -1;
        }

        for (int slotIndex = 0; slotIndex < _companionIds.Length; slotIndex++)
        {
            if (_companionIds[slotIndex] == companionId)
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private int FindEmptyCompanionSlotIndex()
    {
        for (int slotIndex = 0; slotIndex < _companionIds.Length; slotIndex++)
        {
            if (string.IsNullOrWhiteSpace(_companionIds[slotIndex]))
            {
                return slotIndex;
            }
        }

        return -1;
    }

    private bool ContainsCompanion(string companionId)
    {
        bool containsCompanion = FindCompanionSlotIndex(companionId) >= 0;
        return containsCompanion;
    }

    private bool IsValidCompanionSlotIndex(int slotIndex)
    {
        bool isValidSlotIndex = slotIndex >= 0 && slotIndex < _companionIds.Length;
        return isValidSlotIndex;
    }

    private void OnCompanionSlotChanged(int slotIndex)
    {
        CompanionSlotChanged?.Invoke(slotIndex);
    }
}