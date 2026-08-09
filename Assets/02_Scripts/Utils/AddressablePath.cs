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
}
