

public enum ServerErrorCode
{
    Success = 0,

    InvalidCredentials = 1001,
    TokenExpired = 1002,
    InvalidToken = 1003,
    UserNotFound = 1004,
    Unauthorized = 1005,

    DatabaseError = 2001,
    QueryFailed = 2002,
    DatabaseConnectionFailed = 2003,

    InsufficientGold = 3001,
    ItemNotFound = 3002,
    AlreadyPurchased = 3003,
    InsufficientExp = 3004,
    LevelUpFailed = 3005,

    InvalidRequest = 4001,
    EmailAlreadyExists = 4002,
    NicknameAlreadyExists = 4003,
    InvalidPassword = 4004,

    InternalServerError = 5001
}
