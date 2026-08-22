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
    }

    public static class Audio
    {
        public const string BGM_LOBBY = "Audio/BGM/Lobby";
        public const string BUTTON_CLICK = "Audio/SFX/ButtonClick";

        public const string TYPING_1 = "Audio/SFX/Typing1";
        public const string TYPING_2 = "Audio/SFX/Typing2";
        public const string TYPING_3 = "Audio/SFX/Typing3";
        public const string TYPING_4 = "Audio/SFX/Typing4";
    }
}