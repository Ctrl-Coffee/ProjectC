using System;

[Serializable]
public class SaveCurrencyResponse
{
    public int Result;
    public string Message;
}

[Serializable]
public class LoadCurrencyResponse
{
    public int Result;
    public string Message;
    public CurrencyDto Data;
}