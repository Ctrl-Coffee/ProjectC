using System;
using System.Collections.Generic;

public struct AwayReport
{
    public TimeSpan AwayDuration;

    public long Money;
    public long DreamPoint;
    public long Energy;

    public Dictionary<string, int> CompletedWorkCounts;

    public int CompletedWorkTotal;

    public bool HasAnything
    {
        get
        {
            return 0 < Money || 0 < DreamPoint || 0 < Energy;
        }
    }
}
