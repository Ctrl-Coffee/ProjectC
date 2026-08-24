using Cysharp.Threading.Tasks;
using UnityEngine;

// TODO 희준 : 영지 로비(길드/대장간) 버튼이 생기면 이 스크립트는 삭제하고 로비 버튼에 OpenGachaView를 연결한다.
public class TestGachaUI : MonoBehaviour
{
    [SerializeField] private long _testScrollAmount = 1000;
    private MiniGameFlowHandler _workHandler = new();

    [ContextMenu("가챠 UI 열기")]
    private void OpenGachaUI()
    {
        OpenGachaUIAsync().Forget();
    }

    private async UniTask OpenGachaUIAsync()
    {
        GachaView view = await GameManager.UI.OpenGachaView();

        if (view == null)
        {
            Debug.LogError("GachaView를 열지 못했습니다.");
        }
    }

    [ContextMenu("몽상의 스크롤 지급")]
    private void AddTestDreamScroll()
    {
        GameManager.Session.Currency.AddDreamScroll(_testScrollAmount);
        Debug.Log($"몽상의 스크롤 지급. 현재 보유: {GameManager.Session.Currency.DreamScroll}");
    }

    [ContextMenu("몽상의 스크롤 확인")]
    private void LogDreamScroll()
    {
        Debug.Log($"몽상의 스크롤 보유: {GameManager.Session.Currency.DreamScroll}");
    }

    [ContextMenu("주사위 게임 진입")]
    private void PlayDiceGamble()
    {
        WorkData data = GameManager.DataTable.GetWorkData("Work_Manual_03");
        if (data == null)
        {
            Debug.LogError($"data가 null {data.Id}");
            return;
        }
        _workHandler.StartMiniGameAsync(data).Forget();
    }
}
