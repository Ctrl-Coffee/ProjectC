using UnityEngine;

public static class UIManagerExtension
{
    public static void OpenTestHUDUI(this UIManager uiManager)
    {
        uiManager.OpenUI<TestHUDUI>(UIRootType.Hud);
    }

    public static void OpenTestPopupUI(this UIManager uiManager)
    {
        uiManager.OpenUI<TestPopupUI>(UIRootType.Popup);
    }
}
