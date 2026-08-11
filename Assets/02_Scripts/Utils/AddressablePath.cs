public static class AddressablePath
{
    public static class Prefab
    {
        public const string UIRoot = "UI/UIRoot";
    }

    public static string GetUIPath(System.Type uiType)
    {
        return $"UI/{uiType.Name}";
    }

    public static class Label
    {
        public const string Common = "Common";
        public const string Reality = "Reality";
        public const string Dream = "Dream";
    }
}
