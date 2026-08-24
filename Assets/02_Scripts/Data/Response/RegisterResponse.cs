using System;


[Serializable]
public class RegisterResponse
{
    public int Result;
    public long UserId;
    public string Email;
    public string Nickname;
    public string CreatedAt;
    public string Message;
}