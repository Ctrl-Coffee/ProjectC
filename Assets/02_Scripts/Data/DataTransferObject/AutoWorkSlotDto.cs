using System.Collections.Generic;

[System.Serializable]
public class AutoWorkSlotDto
{
    public string workId;
    public long startTicks;
    public long endTicks;
}

[System.Serializable]
public class AutoWorkSlotWrapperDto
{
    public List<AutoWorkSlotDto> slots;
}