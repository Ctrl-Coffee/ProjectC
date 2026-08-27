using System;

[Serializable]
public class SaveCurrencyRequest : AuthenticatedRequest
{
    public CurrencyDto currencyData;
}