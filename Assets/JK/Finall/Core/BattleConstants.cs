public static class BattleConstants
{
    public const int INVALID_BATTLE_POSITION = -1;

    public const int MAX_PLAYER_COUNT = 3;
    public const int MAX_ENEMY_COUNT = 3;

    public const int MAX_COMPANION_COUNT = 2;

    public const int HERO_BATTLE_POSITIONS = 1;
    public static readonly int[] COMPANION_BATTLE_POSITIONS = { 0, 2 };
}

public static class AnimationConstants
{
    public const int BASE_LAYER = 0;

    public const string IDLE = "Idle";
    public const string BASIC_ATTACK = "BasicAttack";
    public const string SIGNATURE = "Signature";
    public const string DEATH = "Death";
}