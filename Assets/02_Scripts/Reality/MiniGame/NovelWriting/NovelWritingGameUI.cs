using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NovelWritingGameUI : MiniGameBase
{
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private float _roundTimeLimit = 10f;
    [SerializeField] private UIButtonComponent _catchButton;
    [SerializeField] private UIButtonComponent _stopButton;
    [SerializeField] private RectTransform _barRect;
    [SerializeField] private RectTransform _zoneRect;
    [SerializeField] private TextMeshProUGUI _keyText;

    private float startTime;
    private bool _isKeyMoving;
    private float _roundStartTime;
    private float _currentKeyWidth;
    private UniTaskCompletionSource _stopRequestedSource;
    private UniTaskCompletionSource _catchRequestedSource;
    private NovelCatchLogic _logic = new();

    private void Update()
    {
        if (_isKeyMoving == false) return;

        float elapsed = Time.unscaledTime - _roundStartTime;
        float center = _logic.GetKeyCenter(elapsed, _currentKeyWidth);

        _keyText.rectTransform.anchorMin = new Vector2(center, 0.5f);
        _keyText.rectTransform.anchorMax = new Vector2(center, 0.5f);
        _keyText.rectTransform.anchoredPosition = Vector2.zero;
    }

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        _contentRoot.SetActive(true);

        int roundCount = 0;
        int successCount = 0;
        
        while (true)
        {
            Logger.Log($"3초 후 라운드 시작...");
            bool isContinue = await WaitForNextRoundAsync(token);
            if (isContinue == false) break;
            roundCount++;
            // TODO 희준 : 여기서 라운드 에너지 차감 (FlowHandler 협의 후)

            string keyText = _logic.CreateKeyText();
            _keyText.text = keyText;
            float keyWidth = _keyText.preferredWidth / _barRect.rect.width;

            CatchZone zone = _logic.CreateZone(keyWidth);
            ShowZone(zone);
            _roundStartTime = Time.unscaledTime;
            _currentKeyWidth = keyWidth;
            _isKeyMoving = true;

            bool isCatched = await WaitForCatchAsync(token);

            if (isCatched)
            {
                float elapsed = Time.unscaledTime - startTime;
                float center = _logic.GetKeyCenter(elapsed, keyWidth);
                bool isSuccess = _logic.Judge(center - (keyWidth / 2), center + (keyWidth / 2), zone);

                if(isSuccess)
                {
                    successCount++;
                }

                Logger.Log($"키 중심 {center:F2}, 영역 {zone.Left:F2}~{zone.Right:F2} → {(isSuccess ? "성공" : "실패")}");
            }
            else
            {
                Logger.Log("시간 초과 — 실패");
            }

            _isKeyMoving = false;

        }

        Logger.Log($"정산 — {roundCount}라운드 중 {successCount}회 성공");
        return MiniGameResult.Canceled;   // 임시 — 정산 조각에서 교체
    }

    protected override void ClearGame()
    {
        if (null == _contentRoot) return;

        _contentRoot.SetActive(false);
    }

    private async UniTask<bool> WaitForCatchAsync(CancellationToken token)
    {
        _catchRequestedSource = new UniTaskCompletionSource();
        _catchButton.BindButtonEvent(OnClickCatch);

        try
        {
            int winner = await UniTask.WhenAny(_catchRequestedSource.Task.AttachExternalCancellation(token), UniTask.Delay((int)(_roundTimeLimit * 1000), ignoreTimeScale: true, cancellationToken: token));

            return winner == 0;
        }
        finally
        {
            _catchButton.UnBindButtonAllEvent();
            _catchRequestedSource = null;
        }
    }
    private async UniTask<bool> WaitForNextRoundAsync(CancellationToken token)
    {
        _stopRequestedSource = new UniTaskCompletionSource();
        _stopButton.BindButtonEvent(OnClickStop);

        try
        {
            int winner = await UniTask.WhenAny(_stopRequestedSource.Task.AttachExternalCancellation(token), UniTask.Delay((3000), ignoreTimeScale: true, cancellationToken: token));

            return winner == 1;
        }
        finally
        {
            _stopButton.UnBindButtonAllEvent();
            _stopRequestedSource = null;
        }
    }
    private void OnClickCatch()
    {
        _catchRequestedSource?.TrySetResult();
    }
    private void OnClickStop()
    {
        _stopRequestedSource?.TrySetResult();
    }

    private void ShowZone(CatchZone zone)
    {
        _zoneRect.anchorMin = new Vector2(zone.Left, 0f);
        _zoneRect.anchorMax = new Vector2(zone.Right, 1f);
        _zoneRect.offsetMin = Vector2.zero;
        _zoneRect.offsetMax = Vector2.zero;
    }
}
