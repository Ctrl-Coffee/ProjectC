using System.Collections.Generic;

public class WorkState
{
    private List<AutoWorkSlot> _autoWorkSlots = new();

    public List<AutoWorkSlot> AutoWorkSlots
    {
        get
        {
            return _autoWorkSlots;
        }
    }

    public long LastEnergyRecoverTicks { get; set; }
}
