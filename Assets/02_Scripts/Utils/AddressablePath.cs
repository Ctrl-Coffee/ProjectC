public static class AddressablePath
{
    public static class Prefab
    {
        public const string UIROOT = "UI/UIRoot";
        public const string REAL_LOBBY_BACKGROUND = "RealLobbyBackground";
        public const string DREAM_LOBBY_BACKGROUND = "DreamLobbyBackground";
    }

    public static string GetUIPath(System.Type uiType)
    {
        return $"UI/{uiType.Name}";
    }

    public static class Label
    {
        public const string COMMON = "Common";
        public const string REALITY = "Reality";
        public const string DREAM = "Dream";
        public const string LOADDING = "Loading";
        public const string LOGIN = "Login";
    }

    public static class Audio
    {
        public const string BGM_LOBBY = "Audio/BGM/Lobby";
        public const string BUTTON_CLICK = "Audio/SFX/ButtonClick";

        public const string TYPING_1 = "Audio/SFX/Typing1";
        public const string TYPING_2 = "Audio/SFX/Typing2";
        public const string TYPING_3 = "Audio/SFX/Typing3";
        public const string TYPING_4 = "Audio/SFX/Typing4";
        public const string GACHA_SUMMON = "Audio/SFX/SummonGacha";
        public const string GACHA_SLOT = "Audio/SFX/GachaSlot";
        public const string SUBTITLE = "Audio/SFX/Subtitle";
        public const string DICE_ROLLING = "Audio/SFX/DiceRolling";
        public const string STAMP_SUCCESS = "Audio/SFX/StampSuccess";
        public const string STAMP_FAIL = "Audio/SFX/StampFail";
        public const string PERK_ACTIVE = "Audio/SFX/PerkActive";
        public const string PERK_DEACTIVE = "Audio/SFX/PerkDeactive";
        public const string GAUGE_MOVE = "Audio/SFX/GaugeMove";
    }

    public static class Sprite
    {
        public const string LOGIN_PASSWORD_VIEW_ON = "Sprite/ViewOn";
        public const string LOGIN_PASSWORD_VIEW_OFF = "Sprite/ViewOff";
    }
}