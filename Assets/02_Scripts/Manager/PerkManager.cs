using System.Collections.Generic;

public class PerkManager
{
    private const string PARENT_MODE_ALL = "All";
    private const string PARENT_MODE_NONE = "None";

    private Dictionary<string, List<string>> _childIdsByParent;

    public IReadOnlyList<string> GetUnlockedPerkIds()
    {
        return GameManager.User.UnlockedPerkIds;
    }

    public bool IsUnlocked(string perkId)
    {
        if (string.IsNullOrEmpty(perkId))
            return false;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null != data && IsRoot(data))
            return true;

        return GameManager.User.UnlockedPerkIds.Contains(perkId);
    }

    public PerkNodeState GetState(string perkId)
    {
        if (IsUnlocked(perkId))
            return PerkNodeState.Unlocked;

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkId);

        if (null == data)
            return PerkNodeState.Locked;

        if (!IsParentSatisfied(data))
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

        if (!IsParentSatisfied(data))
        {
            reason = "선행 퍽을 먼저 활성화해야 합니다.";
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
        GameManager.Save.Save();

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

        string brokenChildId = FindBrokenChild(perkId);

        if (!string.IsNullOrEmpty(brokenChildId))
        {
            reason = $"이 퍽이 없으면 조건이 깨지는 퍽이 있습니다. ({brokenChildId})";
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
        GameManager.User.Currency.AddInspiration(data.InspirationCost);
        GameManager.Save.Save();

        return true;
    }

    private bool IsRoot(PerkNodeData data)
    {
        return data.ParentMode == PARENT_MODE_NONE;
    }

    private bool IsParentSatisfied(PerkNodeData data)
    {
        return IsParentSatisfied(data, string.Empty);
    }

    private bool IsParentSatisfied(PerkNodeData data, string excludedParentId)
    {
        if (IsRoot(data))
        {
            return true;
        }

        if (null == data.ParentId)
        {
            return true;
        }

        bool isAllMode = data.ParentMode == PARENT_MODE_ALL;
        int parentCount = 0;

        for (int i = 0; i < data.ParentId.Length; i++)
        {
            string parentId = data.ParentId[i];

            if (string.IsNullOrEmpty(parentId))
            {
                continue;
            }

            parentCount++;

            bool isParentUnlocked = parentId != excludedParentId && IsUnlocked(parentId);

            if (isAllMode && !isParentUnlocked)
            {
                return false;
            }

            if (!isAllMode && isParentUnlocked)
            {
                return true;
            }
        }

        if (parentCount == 0)
        {
            return true;
        }

        return isAllMode;
    }

    private bool IsExclusiveBlocked(PerkNodeData data, out string blockerId)
    {
        blockerId = string.Empty;

        if (string.IsNullOrEmpty(data.ExclusiveGroup))
        {
            return false;
        }

        List<string> unlockedIds = GameManager.User.UnlockedPerkIds;

        for (int i = 0; i < unlockedIds.Count; i++)
        {
            string unlockedId = unlockedIds[i];

            if (unlockedId == data.Id)
            {
                continue;
            }

            PerkNodeData other = GameManager.DataTable.GetPerkNodeData(unlockedId);

            if (null == other)
            {
                continue;
            }

            if (other.ExclusiveGroup == data.ExclusiveGroup)
            {
                blockerId = unlockedId;
                return true;
            }
        }

        return false;
    }

    private string FindBrokenChild(string perkId)
    {
        EnsureChildIndex();

        if (!_childIdsByParent.TryGetValue(perkId, out List<string> childIds))
        {
            return string.Empty;
        }

        for (int i = 0; i < childIds.Count; i++)
        {
            string childId = childIds[i];

            if (!IsUnlocked(childId))
            {
                continue;
            }

            PerkNodeData childData = GameManager.DataTable.GetPerkNodeData(childId);

            if (null == childData)
            {
                continue;
            }

            if (!IsParentSatisfied(childData, perkId))
            {
                return childId;
            }
        }

        return string.Empty;
    }

    private void EnsureChildIndex()
    {
        if (null != _childIdsByParent)
        {
            return;
        }

        _childIdsByParent = new Dictionary<string, List<string>>();

        foreach (KeyValuePair<string, PerkNodeData> pair in GameManager.DataTable.PerkNodeDataTable)
        {
            PerkNodeData data = pair.Value;

            if (null == data.ParentId)
            {
                continue;
            }

            for (int i = 0; i < data.ParentId.Length; i++)
            {
                string parentId = data.ParentId[i];

                if (string.IsNullOrEmpty(parentId))
                {
                    continue;
                }

                if (!_childIdsByParent.TryGetValue(parentId, out List<string> childIds))
                {
                    childIds = new List<string>();
                    _childIdsByParent.Add(parentId, childIds);
                }

                childIds.Add(data.Id);
            }
        }
    }
}
