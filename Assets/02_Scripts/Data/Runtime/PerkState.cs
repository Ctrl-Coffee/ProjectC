using System.Collections.Generic;

public class PerkState
{
    private List<string> _unlockedPerkIds = new();

    public List<string> UnlockedPerkIds
    {
        get
        {
            return _unlockedPerkIds;
        }
    }
}
