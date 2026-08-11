using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public CurrencyModel Currency = new();
    public List<AutoWorkSlot> AutoWorkSlots = new();
}
