using System;
using System.Collections.Generic;

public class PerkManager
{
    private const string PARENT_MODE_ALL = "All";
    private const string PARENT_MODE_NONE = "None";

    public event Action OnPerkChanged;

    private Dictionary<string, List<string>> _neighborIds;

    private HashSet<string> _unlockedSet;

    private PerkStatCalculator _statCalculator = new();
    private PerkUnlockChecker _unlockChecker = new();

    public PerkStatCalculator Stat
    {
        get
        {
            return _statCalculator;
        }
    }

    public PerkUnlockChecker Unlock
    {
        get
        {
            return _unlockChecker;
        }
    }

    private HashSet<string> UnlockedSet
    {
        get
        {
            if (null == _unlockedSet)
            {
                _unlockedSet = new HashSet<string>(GameManager.User.UnlockedPerkIds);
            }

            return _unlockedSet;
        }
    }

    public IReadOnlyList<string> GetUnlockedPerkIds()
    {
        return GameManager.User.UnlockedPerkIds;
    }

    public void InvalidateCache()
    {
        _unlockedSet = null;
        _statCalculator.Invalidate();
        _unlockChecker.Invalidate();
    }

    public bool IsUnlocked(string perkId)
    {
        return IsUnlockedIn(perkId, UnlockedSet);
    }

    public PerkNodeState GetState(string perkId)
    {
        if (IsUnlocked(perkId))
            return PerkNodeState.Unlocked;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data)
            return PerkNodeState.Locked;

        if (!IsRequirementMet(data, UnlockedSet))
            return PerkNodeState.Locked;

        if (IsExclusiveBlocked(data, out string _))
            return PerkNodeState.Locked;

        return PerkNodeState.Unlockable;
    }

    public bool CanUnlock(string perkId, out string reason)
    {
        reason = string.Empty;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data)
        {
            reason = "테이블에 없는 퍽입니다.";
            return false;
        }

        if (IsUnlocked(perkId))
        {
            reason = "이미 활성화된 퍽입니다.";
            return false;
        }

        if (!IsRequirementMet(data, UnlockedSet))
        {
            reason = "이어진 퍽을 먼저 활성화해야 합니다.";
            return false;
        }

        if (IsExclusiveBlocked(data, out string blockerId))
        {
            reason = $"같은 계열의 다른 퍽을 이미 골랐습니다. ({blockerId})";
            return false;
        }

        if (GameManager.User.Currency.Inspiration < data.InspirationCost)
        {
            reason = "영감이 부족합니다.";
            return false;
        }

        return true;
    }

    public bool TryUnlock(string perkId)
    {
        if (!CanUnlock(perkId, out string reason))
        {
            Logger.LogWarning($"id: {perkId}, {reason}");
            return false;
        }

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (0 < data.InspirationCost && !GameManager.User.Currency.TrySpendInspiration(data.InspirationCost))
        {
            Logger.LogWarning($"영감 차감에 실패했습니다. id: {perkId}");
            return false;
        }

        GameManager.User.UnlockedPerkIds.Add(perkId);
        InvalidateCache();
        GameManager.Save.Save();

        GameManager.User.Currency.NotifyMaxEnergyChanged();
        OnPerkChanged?.Invoke();

        return true;
    }

    public bool CanRefund(string perkId, out string reason)
    {
        reason = string.Empty;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data)
        {
            reason = "테이블에 없는 퍽입니다.";
            return false;
        }

        if (IsRoot(data))
        {
            reason = "시작 노드는 취소할 수 없습니다.";
            return false;
        }

        if (!IsUnlocked(perkId))
        {
            reason = "활성화되지 않은 퍽입니다.";
            return false;
        }

        HashSet<string> remain = new HashSet<string>(UnlockedSet);
        remain.Remove(perkId);

        string disconnectedId = FindDisconnectedPerk(remain);

        if (!string.IsNullOrEmpty(disconnectedId))
        {
            reason = $"이 퍽이 없으면 연결이 끊기는 퍽이 있습니다. ({disconnectedId})";
            return false;
        }

        return true;
    }

    public bool TryRefund(string perkId)
    {
        if (!CanRefund(perkId, out string reason))
        {
            Logger.LogWarning($"퍽을 취소할 수 없습니다. id: {perkId}, 사유: {reason}");
            return false;
        }

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        GameManager.User.UnlockedPerkIds.Remove(perkId);
        InvalidateCache();
        GameManager.User.Currency.AddInspiration(data.InspirationCost);
        GameManager.Save.Save();

        GameManager.User.Currency.NotifyMaxEnergyChanged();
        OnPerkChanged?.Invoke();

        return true;
    }

    private bool IsRoot(PerkNodeData data)
    {
        return data.ParentMode == PARENT_MODE_NONE;
    }

    private bool IsUnlockedIn(string perkId, HashSet<string> unlockedIds)
    {
        if (string.IsNullOrEmpty(perkId))
            return false;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null != data && IsRoot(data))
            return true;

        return unlockedIds.Contains(perkId);
    }

    private bool IsRequirementMet(PerkNodeData data, HashSet<string> unlockedIds)
    {
        if (IsRoot(data))
            return true;

        if (data.ParentMode == PARENT_MODE_ALL)
            return IsAllParentUnlocked(data, unlockedIds);

        EnsureIndex();

        if (!_neighborIds.TryGetValue(data.Id, out List<string> neighbors))
            return false;

        for (int i = 0; i < neighbors.Count; i++)
        {
            if (IsUnlockedIn(neighbors[i], unlockedIds))
                return true;
        }

        return false;
    }

    private bool IsAllParentUnlocked(PerkNodeData data, HashSet<string> unlockedIds)
    {
        if (null == data.ParentId)
            return true;

        for (int i = 0; i < data.ParentId.Length; i++)
        {
            string parentId = data.ParentId[i];

            if (string.IsNullOrEmpty(parentId))
                continue;

            if (!IsUnlockedIn(parentId, unlockedIds))
                return false;
        }

        return true;
    }

    private bool IsExclusiveBlocked(PerkNodeData data, out string blockerId)
    {
        blockerId = string.Empty;

        if (string.IsNullOrEmpty(data.ExclusiveGroup))
            return false;

        List<string> unlockedList = GameManager.User.UnlockedPerkIds;

        for (int i = 0; i < unlockedList.Count; i++)
        {
            if (unlockedList[i] == data.Id)
                continue;

            PerkNodeData other = GameManager.DataTable.GetPerkNodeData(unlockedList[i]);

            if (null == other)
                continue;

            if (other.ExclusiveGroup == data.ExclusiveGroup)
            {
                blockerId = unlockedList[i];
                return true;
            }
        }

        return false;
    }

    private string FindDisconnectedPerk(HashSet<string> unlockedIds)
    {
        HashSet<string> connected = new HashSet<string>();

        bool isAdded = true;

        while (isAdded)
        {
            isAdded = false;

            foreach (string id in unlockedIds)
            {
                if (connected.Contains(id))
                    continue;

                PerkNodeData data = GameManager.DataTable.GetPerkNodeData(id);

                if (null == data)
                    continue;

                if (!IsRequirementMet(data, connected))
                    continue;

                connected.Add(id);
                isAdded = true;
            }
        }

        foreach (string id in unlockedIds)
        {
            if (!connected.Contains(id))
                return id;
        }

        return string.Empty;
    }

    private void EnsureIndex()
    {
        if (null != _neighborIds)
            return;

        _neighborIds = new Dictionary<string, List<string>>();

        foreach (KeyValuePair<string, PerkNodeData> pair in GameManager.DataTable.PerkNodeDataTable)
        {
            PerkNodeData data = pair.Value;

            if (null == data.ParentId)
                continue;

            for (int i = 0; i < data.ParentId.Length; i++)
            {
                string parentId = data.ParentId[i];

                if (string.IsNullOrEmpty(parentId))
                    continue;

                AddNeighbor(data.Id, parentId);
                AddNeighbor(parentId, data.Id);
            }
        }
    }

    private void AddNeighbor(string perkId, string neighborId)
    {
        if (!_neighborIds.TryGetValue(perkId, out List<string> neighbors))
        {
            neighbors = new List<string>();
            _neighborIds.Add(perkId, neighbors);
        }

        if (!neighbors.Contains(neighborId))
        {
            neighbors.Add(neighborId);
        }
    }
}
