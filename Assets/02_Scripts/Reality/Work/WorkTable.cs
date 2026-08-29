using System.Collections.Generic;

public static class WorkTable
{
    public static IReadOnlyList<WorkData> GetList(WorkType workType)
    {
        List<WorkData> list = new();

        foreach (WorkData data in GameManager.DataTable.WorkDataTable.Values)
        {
            if (workType != data.Type)
            {
                continue;
            }

            if (!GameManager.Perk.Unlock.IsUnlocked(data.Id))
            {
                continue;
            }

            list.Add(data);
        }

        return list;
    }

    public static bool HasAny(WorkType workType)
    {
        foreach (WorkData data in GameManager.DataTable.WorkDataTable.Values)
        {
            if (workType != data.Type)
            {
                continue;
            }

            if (!GameManager.Perk.Unlock.IsUnlocked(data.Id))
            {
                continue;
            }

            return true;
        }

        return false;
    }
}
