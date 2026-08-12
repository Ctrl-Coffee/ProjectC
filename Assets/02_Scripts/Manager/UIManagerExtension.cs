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
    public static void OpenLobbyHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<LobbyUI>().Forget();
    }

    public static void ExampleVoidFunc(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<TestPopupUI>().Forget();
    }

    public static UniTask<TestPopupUI> ExampleAsyncFunc(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<TestPopupUI>();
    }

}
