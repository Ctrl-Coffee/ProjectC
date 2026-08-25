using System;

[Serializable]
public class SaveCurrencyResponse
{
    public int result;
    public string message;
}

[Serializable]
public class LoadCurrencyResponse
{
    public int result;
    public string message;
    public CurrencyDto data;
}