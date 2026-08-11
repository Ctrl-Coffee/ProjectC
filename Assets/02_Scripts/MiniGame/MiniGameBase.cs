using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;

public abstract class MiniGameBase : UIBase
{
    [Header("슬라이드 연출")]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private float _slideInDuration = 0.6f;
    [SerializeField] private float _slideOutDuration = 0.35f;
    [SerializeField] private float _slideDistance = 0f;

    // TODO: 김경훈 (26.08.11) - 결과창 생성/표시를 UIManager로 이관 후 삭제
    [Header("결과창")]
    [SerializeField] private MiniGameResultUI _resultUIPrefab;

    private MiniGameResultUI _resultUIInstance;

    private Vector2 _shownPosition;
    private bool _isPositionCaptured = false;

    public bool IsPlaying { get; private set; }

    protected RectTransform Panel
    {
        get
        {
            if (null == _panel)
            {
                _panel = this.transform as RectTransform;
            }

            return _panel;
        }
    }

    protected virtual void Awake()
    {
        CapturePosition();
        this.gameObject.SetActive(false);
    }

    public async UniTask<MiniGameResult> RunAsync(MiniGameContext context, CancellationToken token)
    {
        if (null == Panel)
        {
            Logger.LogError("RectTransform을 찾을 수 없습니다.");
            return MiniGameResult.Canceled;
        }

        if (IsPlaying)
        {
            Logger.LogWarning("이미 진행 중입니다.");
            return MiniGameResult.Canceled;
        }

        IsPlaying = true;

        try
        {
            CapturePosition();

            Panel.DOKill();
            this.gameObject.SetActive(true);

            Vector2 hiddenPosition = GetHiddenPosition();
            Panel.anchoredPosition = hiddenPosition;

            await SlideAsync(_shownPosition, _slideInDuration, token);

            MiniGameResult result = await PlayAsync(context, token);

            if (result.IsCompleted)
            {
                await ShowResultAsync(result, token);
            }

            await SlideAsync(hiddenPosition, _slideOutDuration, token);

            return result;
        }
        catch (OperationCanceledException)
        {
            return MiniGameResult.Canceled;
        }
        finally
        {
            IsPlaying = false;
            HideImmediate();
        }
    }

    private void HideImmediate()
    {
        if (null == this)
        {
            return;
        }

        if (null != _panel)
        {
            _panel.DOKill();
        }

        this.gameObject.SetActive(false);
    }

    public abstract UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token);
    public abstract UniTask PlaySkipAsync(MiniGameContext context, CancellationToken token);

    // TODO: 김경훈 (26.08.11) - 결과창 생성/표시를 UIManager로 이관 후 삭제
    #region Moved to UIManager
    private async UniTask ShowResultAsync(MiniGameResult result, CancellationToken token)
    {
        MiniGameResultUI resultUI = EnsureResultUI();

        if (null == resultUI)
        {
            return;
        }

        await resultUI.ShowAsync(result, token);
    }

    private MiniGameResultUI EnsureResultUI()
    {
        if (null != _resultUIInstance)
        {
            return _resultUIInstance;
        }

        if (null == _resultUIPrefab)
        {
            Logger.LogWarning("ResultUIPrefab이 없어 결과창을 건너뜁니다.");
            return null;
        }

        _resultUIInstance = Instantiate(_resultUIPrefab, Panel, false);

        return _resultUIInstance;
    }

    #endregion

    public override void PlayOpenAnimation()
    {
        CapturePosition();

        Panel.DOKill();
        Panel.anchoredPosition = GetHiddenPosition();
    }

    public override Tween PlayCloseAnimation()
    {
        CapturePosition();

        Panel.DOKill();

        return Panel.DOAnchorPos(GetHiddenPosition(), _slideOutDuration).SetEase(Ease.InCubic).SetUpdate(true);
    }

    private async UniTask SlideAsync(Vector2 to, float duration, CancellationToken token)
    {
        if (duration <= 0f)
        {
            Panel.anchoredPosition = to;
            return;
        }

        await Panel.DOAnchorPos(to, duration).SetEase(Ease.OutCubic).SetUpdate(true).ToUniTask(cancellationToken: token);
    }

    private void CapturePosition()
    {
        if (_isPositionCaptured)
        {
            return;
        }

        if (null == Panel)
        {
            return;
        }

        _shownPosition = Panel.anchoredPosition;
        _isPositionCaptured = true;
    }

    private Vector2 GetHiddenPosition()
    {
        float distance = _slideDistance;

        if (distance <= 0f)
        {
            distance = Panel.rect.height;
        }

        if (distance <= 0f)
        {
            distance = Screen.height;
        }

        return new Vector2(_shownPosition.x, _shownPosition.y - distance);
    }
}
