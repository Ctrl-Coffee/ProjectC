using System;


[Serializable]
public class RegisterResponse
{
    public int result;
    public long userId;
    public string email;
    public string nickname;
    public string createdAt;
    public string message;
}