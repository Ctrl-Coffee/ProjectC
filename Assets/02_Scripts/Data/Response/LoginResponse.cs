using System;

[Serializable]
public class LoginResponse : CommonResponse
{
    public long userId;
    public string token;
}