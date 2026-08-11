using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WorkTestUI : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _startButton;

    [SerializeField] private MiniGameBase _miniGamePrefab;

    // TODO: 김경훈 (26.08.11) - UI 계층 확정되면 삭제. UIManager가 Popup 캔버스를 결정하도록 변경
    [SerializeField] private RectTransform _miniGameParent;

    [Header("테스트 업무 정보")]
    [SerializeField] private string _workId = "TestWork";
    [SerializeField] private int _workLevel = 1;

    [Header("결과 표시 (선택)")]
    [SerializeField] private TextMeshProUGUI _resultText;

    // TODO: 김경훈 (26.08.10) - UIManager를 통해 미니게임을 관리하도록 변경 후 삭제
    private MiniGameBase _miniGameInstance;

    private void OnEnable()
    {
        if (null == _startButton)
        {
            Logger.LogError("StartButton이 연결되지 않았습니다.");
            return;
        }

        _startButton.BindButtonEvent(OnClickStartMiniGame);
    }

    private void OnDisable()
    {
        if (null == _startButton)
        {
            return;
        }

        _startButton.UnBindAllButtonEvent();
    }

    private void OnClickStartMiniGame()
    {
        StartMiniGameAsync().Forget();
    }

    private async UniTaskVoid StartMiniGameAsync()
    {
        MiniGameBase miniGame = EnsureMiniGame();

        if (null == miniGame)
        {
            return;
        }

        if (miniGame.IsPlaying)
        {
            return;
        }

        SetResultText("진행 중...");

        MiniGameContext context = new MiniGameContext
        {
            WorkId = _workId,
            WorkLevel = _workLevel,
        };

        MiniGameResult result = await miniGame.RunAsync(context, this.GetCancellationTokenOnDestroy());

        if (!result.IsCompleted)
        {
            SetResultText("취소됨");
            return;
        }

        string message = $"{result.Grade} / 정확도 {result.Accuracy:P0}";

        SetResultText(message);
        Logger.Log($"미니게임 종료 - {message}");
    }

    private MiniGameBase EnsureMiniGame()
    {
        if (null != _miniGameInstance)
        {
            return _miniGameInstance;
        }

        if (null == _miniGamePrefab)
        {
            Logger.LogError("MiniGamePrefab이 연결되지 않았습니다.");
            return null;
        }

        Transform parent = _miniGameParent;
        if (null == parent)
        {
            parent = this.transform.parent;
        }

        if (null == parent)
        {
            Logger.LogError("미니게임을 생성할 부모를 찾을 수 없습니다.");
            return null;
        }

        _miniGameInstance = Instantiate(_miniGamePrefab, parent, false);
        _miniGameInstance.transform.SetAsLastSibling();

        return _miniGameInstance;
    }

    private void SetResultText(string message)
    {
        if (null == _resultText)
        {
            return;
        }

        _resultText.text = message;
    }
}
