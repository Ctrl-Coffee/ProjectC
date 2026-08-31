public static class Const
{
    // 스텟 관련 
    public const float CRITICAL_DAMAGE_MULTIPLIER = 1.5f;
    public const float PERCENT_TO_RATE = 0.01f;
    public const float RATE_TO_PERCENT = 100f;
    public const int FIRST_BONUS_LEVEL = 2;

    // 계산기 상수
    public const float ATTACK_WEIGHT = 10f;
    public const float CRITICAL_WEIGHT = 0.5f;
    public const float HEALTH_WEIGHT = 1f;
    public const float DEFENSE_WEIGHT = 10f;
    public const float NORMAL_SKILL_HASTE_WEIGHT = 5f;
    public const float SPECIAL_SKILL_HASTE_WEIGHT = 5f;
    public const float HASTE_BASE = 100f;
    public const float MIN_COOLDOWN_RATE = 0.01f;

    // 퍽 관련
    public const float NO_COMPOUND_RATE = 1f;

    // UI 문구, 추후 삭제
    public const string NO_PERK_BUFF = "적용 중인 퍽 효과가 없습니다.";
    public const string MAX_LEVEL_TEXT = "MAX";
    public const string COLOR_GOOD = "#7FD97F";
    public const string COLOR_BAD = "#FF7B7B";

    //전투 편성 관련
    public const int INVALID_BATTLE_POSITION = -1;
    public const int MAX_PLAYER_COUNT = 3;
    public const int MAX_ENEMY_COUNT = 3;
    public const int MAX_COMPANION_COUNT = 2;
    public const int HERO_BATTLE_POSITIONS = 1;
    public static readonly int[] COMPANION_BATTLE_POSITIONS = { 0, 2 };

    //전투 애니메이션 관련
    public const int BASE_LAYER = 0;
    public const string IDLE = "Idle";
    public const string BASIC_ATTACK = "BasicAttack";
    public const string SIGNATURE = "Signature";
    public const string DEATH = "Death";

    //BT 변수명
    public const string SKILL_READY_EVENT = "SkillReadyEvent";
    public const string BASIC_ATTACK_SKILL_READY = "IsBasicAttackSkillReady";
    public const string SIGNATURE_SKILL_READY = "IsSignatureSkillReady";

    //색
    public const string STAGE_DANGER_COLOR = "#D81B60";
    public const string STAGE_SAFE_COLOR = "#222222";
}