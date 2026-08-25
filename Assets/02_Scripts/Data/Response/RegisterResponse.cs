using System;


[Serializable]
public class RegisterResponse : CommonResponse
{
    public long userId;
    public string email;
    public string nickname;
    public string createdAt;
}