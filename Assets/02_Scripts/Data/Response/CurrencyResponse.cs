using System;

[Serializable]
public class SaveCurrencyResponse : CommonResponse
{
}

[Serializable]
public class LoadCurrencyResponse : CommonResponse
{
    public CurrencyDto data;
}