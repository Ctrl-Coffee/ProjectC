using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public CurrencyModel Currency = new();
    public List<AutoWorkSlot> AutoWorkSlots = new();
    public long LastEnergyRecoverTicks;
    public OwnedPlayerData Player = new();

    public void EnsureDefaults()
    {
        if (Player.Level <= 0)
        {
            Player.Level = 1;
        }
    }
}
