using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 완료 시점을 알아야 하나?
/// Yes: Async 함수 - ex) fade, loading, 선택 결과 기다리기, 후속 작업 필요한 ui
/// No: void 함수
/// 예시 함수 참고
/// </summary>
public static class UIManagerExtension
{
    public static void OpenRealHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<RealHudUI>().Forget();
    }

    public static void OpenDreamHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<DreamHudUI>().Forget();
    }

    public static void CloseRealHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<RealHudUI>().Forget();
    }


    public static void CloseDreamHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<DreamHudUI>().Forget();
    }

    public static async void OpenConfirmUI(this UIManager uiManager, ConfirmData confirmData, ConfirmButtonAction buttonAction)
    {
        var ui = await uiManager.OpenPopupUI<ConfirmUI>();
        ui.SetConfirmUI(confirmData, buttonAction);
    }


    public static UniTask<WorkInfoUI> OpenWorkInfoUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<WorkInfoUI>();
    }

    public static UniTask<SubtitleEditGameUI> OpenSubtitleEditGameUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<SubtitleEditGameUI>();
    }

    public static UniTask<MiniGameResultUI> OpenMiniGameResultUI(this UIManager uiManager)
    {
        return uiManager.OpenOverlayUI<MiniGameResultUI>();
    }
}
