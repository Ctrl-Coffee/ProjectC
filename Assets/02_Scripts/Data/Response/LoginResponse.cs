using System;

[Serializable]
public class LoginResponse
{
    public int result;
    public long userId;
    public string token;
    public string message;
}