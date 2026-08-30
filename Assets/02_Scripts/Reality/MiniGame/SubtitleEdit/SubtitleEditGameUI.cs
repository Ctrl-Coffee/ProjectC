using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleEditGameUI : MiniGameBase
{
    [Header("배치")]
    [SerializeField] private RectTransform _playArea;
    [SerializeField] private RectTransform _targetSlot;
    [SerializeField] private Button _btnAttach;

    [SerializeField] private FallingSubtitle _subtitlePrefab;

    // TODO: 김경훈 (26.08.10) 자막 여러개가 되면 PoolManager로 옮기기
    private FallingSubtitle _subtitleInstance;

    [Header("결과 연출")]
    [SerializeField] private int _resultDelayMs = 800;

    [Header("난이도")]
    [SerializeField] private float _fallSpeedRatio = 0.5f;
    [SerializeField] private float _toleranceRatio = 0.1f;
    [SerializeField] private float _perfectRadiusRatio = 0.02f;

    private bool _hasAttachResult = false;
    private float _attachAccuracy = 0f;

    private float FallSpeed
    {
        get
        {
            return _fallSpeedRatio * _playArea.rect.height;
        }
    }

    private float Tolerance
    {
        get
        {
            return _toleranceRatio * _playArea.rect.height;
        }
    }

    private float PerfectRadius
    {
        get
        {
            return _perfectRadiusRatio * _playArea.rect.height;
        }
    }

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (!ValidateReferences())
        {
            return MiniGameResult.Canceled;
        }

        if (!SetupRound())
        {
            return MiniGameResult.Canceled;
        }

        BindInput();

        try
        {
            float accuracy = await RunSubtitleAsync(token);

            return MiniGameResult.Completed(accuracy);
        }
        finally
        {
            UnbindInput();
            HideSubtitle();
        }
    }

    private bool SetupRound()
    {
        if (null == EnsureSubtitle())
        {
            return false;
        }

        _subtitleInstance.SetPosition(GetSubtitleStartPosition());

        _subtitleInstance.gameObject.SetActive(true);

        _hasAttachResult = false;
        _attachAccuracy = 0f;

        return true;
    }

    protected override void ClearGame()
    {
        HideSubtitle();
    }

    private void HideSubtitle()
    {
        if (null == _subtitleInstance)
        {
            return;
        }

        _subtitleInstance.gameObject.SetActive(false);
    }

    private FallingSubtitle EnsureSubtitle()
    {
        if (null != _subtitleInstance)
        {
            return _subtitleInstance;
        }

        if (null == _subtitlePrefab)
        {
            Logger.LogError("SubtitlePrefab이 연결되지 않았습니다.");
            return null;
        }

        _subtitleInstance = Instantiate(_subtitlePrefab, _playArea, false);

        return _subtitleInstance;
    }

    private Vector2 GetSubtitleStartPosition()
    {
        Vector2 startPosition = _targetSlot.anchoredPosition;
        startPosition.y = (_playArea.rect.height * 0.5f) + _subtitleInstance.RectTransform.rect.height;

        return startPosition;
    }

    private async UniTask<float> RunSubtitleAsync(CancellationToken token)
    {
        float fallSpeed = FallSpeed;
        float missLineY = _targetSlot.anchoredPosition.y - Tolerance;

        while (true)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            if (_hasAttachResult)
            {
                // 붙인 자리에 그대로 세워 둔 채로 잠깐 보여 준다. 얼마나 어긋났는지 확인시키는 구간.
                await UniTask.Delay(_resultDelayMs, ignoreTimeScale: true, cancellationToken: token);

                return _attachAccuracy;
            }

            float deltaTime = GetDeltaTime();
            _subtitleInstance.Fall(fallSpeed * deltaTime);

            if (_subtitleInstance.RectTransform.anchoredPosition.y < missLineY)
            {
                return 0f;
            }
        }
    }

    private float EvaluateAttach()
    {
        if (null == _subtitleInstance)
        {
            return 0f;
        }

        float distance = Vector2.Distance(_subtitleInstance.RectTransform.anchoredPosition, _targetSlot.anchoredPosition);

        return MiniGameScore.FromDistance(distance, PerfectRadius, Tolerance);
    }

    private void BindInput()
    {
        if (null == _btnAttach)
        {
            return;
        }

        _btnAttach.onClick.RemoveListener(OnClickAttach);
        _btnAttach.onClick.AddListener(OnClickAttach);
    }

    private void UnbindInput()
    {
        if (null == _btnAttach)
        {
            return;
        }

        _btnAttach.onClick.RemoveListener(OnClickAttach);
    }

    private void OnClickAttach()
    {
        if (_hasAttachResult)
        {
            return;
        }

        _attachAccuracy = EvaluateAttach();
        _hasAttachResult = true;

        GameManager.Sound.PlaySFX(AddressablePath.Audio.SUBTITLE);
    }

    private bool ValidateReferences()
    {
        if (null == _playArea || null == _targetSlot || null == _subtitlePrefab)
        {
            Logger.LogError("참조 비어있음 (PlayArea / TargetSlot / Subtitle)");
            return false;
        }

        if (_playArea.rect.height <= 0f)
        {
            Logger.LogError("PlayArea의 높이가 0.");
            return false;
        }

        if (null == _btnAttach)
        {
            Logger.LogWarning("참조 비어있음 (AttachButton)");
        }

        return true;
    }
}
