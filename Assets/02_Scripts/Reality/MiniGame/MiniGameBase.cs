using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

public abstract class MiniGameBase : UIBase
{
    [Header("슬라이드 연출")]
    [SerializeField] private RectTransform _panel;
    [SerializeField] private float _slideInDuration = 0.6f;
    [SerializeField] private float _slideOutDuration = 0.35f;
    [SerializeField] private float _slideDistance = 0f;

    private Vector2 _shownPosition;
    private Tween _openTween;

    private bool _isPositionCaptured = false;

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
    }

    public async UniTask<MiniGameResult> RunAsync(MiniGameContext context, CancellationToken token)
    {
        if (null == Panel)
        {
            Logger.LogError("RectTransform을 찾을 수 없습니다.");
            return MiniGameResult.Canceled;
        }

        await WaitForOpenAsync(token);

        return await PlayAsync(context, token);
    }

    public abstract UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token);

    public override Tween PlayOpenAnimation()
    {
        CapturePosition();

        Panel.DOKill();
        Panel.anchoredPosition = GetHiddenPosition();

        if (_slideInDuration <= 0f)
        {
            Panel.anchoredPosition = _shownPosition;
            _openTween = null;
            return _openTween;
        }

        _openTween = Panel.DOAnchorPos(_shownPosition, _slideInDuration).SetEase(Ease.OutCubic).SetUpdate(true);
        return _openTween;
    }

    public override Tween PlayCloseAnimation()
    {
        CapturePosition();

        Panel.DOKill();
        _openTween = null;

        return Panel.DOAnchorPos(GetHiddenPosition(), _slideOutDuration).SetEase(Ease.InCubic).SetUpdate(true);
    }

    private async UniTask WaitForOpenAsync(CancellationToken token)
    {
        Tween openTween = _openTween;
        _openTween = null;

        if (null == openTween || !openTween.IsActive())
        {
            return;
        }

        await openTween.ToUniTask(cancellationToken: token);
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
