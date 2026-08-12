using System.Collections.Generic;

public static class WorkTable
{
    public static IReadOnlyList<WorkData> GetList(WorkType workType)
    {
        List<WorkData> list = new();

        foreach (WorkData data in GameManager.DataTable.WorkDataTable.Values)
        {
            if (workType == data.Type)
            {
                list.Add(data);
            }
        }

        return list;
    }
}
