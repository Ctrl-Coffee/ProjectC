using System;

[Serializable]
public class SaveCurrencyRequest : AuthenticatedRequest
{
    public CurrencyDto CurrencyData;
}