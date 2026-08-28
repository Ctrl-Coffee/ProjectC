using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _zoneHeight = 110f;
    [SerializeField] private float _typingInterval = 0.05f;

    private bool _isKeyMoving;
    private float _roundStartTime;
    private float _currentKeyWidth;
    private bool _isCountingDown;
    private float _delayStartTime;
    private const float ROUND_DELAY = 3f;
    private UniTaskCompletionSource _stopRequestedSource;
    private UniTaskCompletionSource _catchRequestedSource;
    private NovelCatchLogic _logic = new();

    private void Update()
    {
        if (_isKeyMoving)
        {
            float elapsed = Time.unscaledTime - _roundStartTime;
            float center = _logic.GetKeyCenter(elapsed, _currentKeyWidth);
            _timerText.text = $"남은시간 : {Mathf.CeilToInt(_roundTimeLimit - elapsed)}"; 
            _keyText.rectTransform.anchorMin = new Vector2(center, 0.5f);
            _keyText.rectTransform.anchorMax = new Vector2(center, 0.5f);
            _keyText.rectTransform.anchoredPosition = Vector2.zero;
        }

        if (_isCountingDown)
        {
            float remaining = ROUND_DELAY - (Time.unscaledTime - _delayStartTime);
            _countdownText.text = Mathf.CeilToInt(remaining).ToString();
        }
    }

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        CancellationTokenSource linkedSource = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy());
        token = linkedSource.Token;
        _contentRoot.SetActive(true);

        int roundCount = 0;
        int successCount = 0;

        try
        {
            while (true)
            {

                Logger.Log($"3초 후 라운드 시작...");
                _delayStartTime = Time.unscaledTime;
                _isCountingDown = true;
                _countdownText.gameObject.SetActive(true);

                bool isContinue = await WaitForNextRoundAsync(token);
                _isCountingDown = false;
                _countdownText.gameObject.SetActive(false);

                if (isContinue == false) break;

                if (GameManager.Session.Currency.TrySpendEnergy(context.EnergyCost) == false)
                {
                    Logger.Log("에너지 부족 — 강제 정산");
                    break;
                }
                roundCount++;

                string keyText = _logic.CreateKeyText();
                _keyText.text = keyText;
                float keyWidth = _keyText.preferredWidth / _barRect.rect.width;

                CatchZone zone = _logic.CreateZone(keyWidth);
                ShowZone(zone);
                _roundStartTime = Time.unscaledTime;
                _currentKeyWidth = keyWidth;

                _isKeyMoving = true;

                NovelRoundResult roundResult = await WaitForCatchAsync(token);
                _isKeyMoving = false;

                if (roundResult == NovelRoundResult.Stop) break;

                if (roundResult == NovelRoundResult.Catch)
                {
                    float elapsed = Time.unscaledTime - _roundStartTime;
                    float center = _logic.GetKeyCenter(elapsed, keyWidth);
                    bool isSuccess = _logic.Judge(center - (keyWidth / 2), center + (keyWidth / 2), zone);

                    if (isSuccess)
                    {
                        GameManager.Sound.PlaySFX(AddressablePath.Audio.NOVEL_WRITING);
                        await PlayTypingAsync(keyText, token);
                        successCount++;
                    }

                    Logger.Log($"키 중심 {center:F2}, 영역 {zone.Left:F2}~{zone.Right:F2} → {(isSuccess ? "성공" : "실패")}");
                }
                else
                {
                    Logger.Log("시간 초과 — 실패");

                }
            }
        }
        catch (OperationCanceledException)
        {
            _isKeyMoving = false;
            Logger.Log("게임 중단 - 성공한 라운드까지 정산");
        }
        finally
        {
            linkedSource.Dispose();
        }

        Logger.Log($"정산 — {roundCount}라운드 중 {successCount}회 성공");
        return MiniGameScore.FromNovel(successCount);
    }
    private async UniTask PlayTypingAsync(string word, CancellationToken token)
    {
        List<string> frames = HangulUtil.BuildTypingFrames(word);

        foreach (string frame in frames)
        {
            _keyText.text = frame;
            await UniTask.Delay((int)(_typingInterval * 1000), ignoreTimeScale: true, cancellationToken: token);
        }
    }


    protected override void ClearGame()
    {
        if (null == _contentRoot) return;

        _contentRoot.SetActive(false);
    }

    private async UniTask<NovelRoundResult> WaitForCatchAsync(CancellationToken token)
    {
        _catchRequestedSource = new UniTaskCompletionSource();
        _stopRequestedSource = new UniTaskCompletionSource();
        _catchButton.BindButtonEvent(OnClickCatch);
        _stopButton.BindButtonEvent(OnClickStop);

        try
        {
            int winner = await UniTask.WhenAny(_catchRequestedSource.Task.AttachExternalCancellation(token), UniTask.Delay((int)(_roundTimeLimit * 1000), ignoreTimeScale: true, cancellationToken: token), _stopRequestedSource.Task.AttachExternalCancellation(token));

            switch (winner)
            {
                case 0: return NovelRoundResult.Catch;
                case 1: return NovelRoundResult.Timeout;
                default: return NovelRoundResult.Stop;

            }
        }
        finally
        {
            _catchButton.UnBindButtonAllEvent();
            _stopButton.UnBindButtonAllEvent();
            _stopRequestedSource = null;
            _catchRequestedSource = null;
        }
    }
    private async UniTask<bool> WaitForNextRoundAsync(CancellationToken token)
    {
        _stopRequestedSource = new UniTaskCompletionSource();
        _stopButton.BindButtonEvent(OnClickStop);

        try
        {
            int winner = await UniTask.WhenAny(_stopRequestedSource.Task.AttachExternalCancellation(token), UniTask.Delay(((int)(ROUND_DELAY * 1000)), ignoreTimeScale: true, cancellationToken: token));

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
        _zoneRect.anchorMin = new Vector2(zone.Left, 0.5f);
        _zoneRect.anchorMax = new Vector2(zone.Right, 0.5f);
        _zoneRect.sizeDelta = new Vector2(0f, _zoneHeight);
        _zoneRect.anchoredPosition = Vector2.zero;
    }
}
