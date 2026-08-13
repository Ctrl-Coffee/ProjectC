using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SubtitleEditGameUI : MiniGameBase
{
    [Header("배치")]
    [SerializeField] private RectTransform _playArea;
    [SerializeField] private RectTransform _targetSlot;
    [SerializeField] private UIButtonComponent _btnAttach;

    [SerializeField] private FallingSubtitle _subtitlePrefab;

    // TODO: 김경훈 (26.08.10) 자막 여러개가 되면 PoolManager로 옮기기
    private FallingSubtitle _subtitleInstance;

    [Header("난이도")]
    [SerializeField] private float _fallSpeedRatio = 0.3f;
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

            return new MiniGameResult
            {
                IsCompleted = true,
                Accuracy = accuracy,
                Grade = MiniGameGradeTable.GetGrade(accuracy),
            };
        }
        finally
        {
            UnbindInput();
        }
    }

    private bool SetupRound()
    {
        if (null == EnsureSubtitle())
        {
            return false;
        }

        _subtitleInstance.SetPosition(GetSubtitleStartPosition());

        _hasAttachResult = false;
        _attachAccuracy = 0f;

        return true;
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

        return CalculateAccuracy(distance, PerfectRadius, Tolerance);
    }

    private float CalculateAccuracy(float distance, float perfectRadius, float tolerance)
    {
        if (tolerance <= 0f)
        {
            return 0f;
        }

        if (distance <= perfectRadius)
        {
            return 1f;
        }

        float falloffRange = tolerance - perfectRadius;

        if (falloffRange <= 0f)
        {
            return 0f;
        }

        float accuracy = 1f - ((distance - perfectRadius) / falloffRange);

        return Mathf.Clamp01(accuracy);
    }

    private void BindInput()
    {
        if (null == _btnAttach)
        {
            return;
        }

        _btnAttach.BindButtonEvent(OnClickAttach);
    }

    private void UnbindInput()
    {
        if (null == _btnAttach)
        {
            return;
        }

        _btnAttach.UnBindButtonAllEvent();
    }

    private void OnClickAttach()
    {
        if (_hasAttachResult)
        {
            return;
        }

        _attachAccuracy = EvaluateAttach();
        _hasAttachResult = true;
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
