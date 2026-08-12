using System.Collections.Generic;

public static class WorkTable
{
    private static List<WorkData> _manualList = new();
    private static List<WorkData> _autoList = new();

    public static void Build(Dictionary<string, WorkData> workDataTable)
    {
        _manualList.Clear();
        _autoList.Clear();

        if (null == workDataTable)
        {
            return;
        }

        foreach (WorkData data in workDataTable.Values)
        {
            if (WorkType.Auto == data.Type)
            {
                _autoList.Add(data);
                continue;
            }

            _manualList.Add(data);
        }
    }

    public static IReadOnlyList<WorkData> GetList(WorkType workType)
    {
        if (WorkType.Auto == workType)
            return _autoList;

        return _manualList;
    }
}
