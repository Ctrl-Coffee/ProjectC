using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SubtitleEditGameUI : MiniGameBase
{
    private const float MIN_FALL_SPEED = 1f;

    [Header("배치")]
    [SerializeField] private RectTransform _playArea;
    [SerializeField] private RectTransform _targetSlot;
    [SerializeField] private UIButtonComponent _attachButton;

    [SerializeField] private FallingSubtitle _subtitlePrefab;

    // TODO: 김경훈 (26.08.10) 자막 여러개가 되면 PoolManager로 옮기기
    private FallingSubtitle _subtitleInstance;

    [Header("난이도 - 기본값")]
    [SerializeField] private float _baseFallSpeed = 400f;
    [SerializeField] private float _baseTolerance = 150f;
    [SerializeField] private float _basePerfectRadius = 30f;

    [Header("난이도 - 업무 레벨당 증감")]
    [SerializeField] private float _fallSpeedPerLevel = 20f;
    [SerializeField] private float _tolerancePerLevel = 5f;
    [SerializeField] private float _minTolerance = 40f;
    [SerializeField] private float _perfectRadiusPerLevel = 1f;
    [SerializeField] private float _minPerfectRadius = 8f;

    private bool _hasAttachResult = false;
    private float _attachAccuracy = 0f;
    private float _currentTolerance = 0f;
    private float _currentPerfectRadius = 0f;

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (!ValidateReferences())
        {
            return MiniGameResult.Canceled;
        }

        float fallSpeed = GetFallSpeed(context.WorkLevel);
        _currentTolerance = GetTolerance(context.WorkLevel);
        _currentPerfectRadius = GetPerfectRadius(context.WorkLevel);

        if (!SetupRound())
        {
            return MiniGameResult.Canceled;
        }

        BindInput();

        try
        {
            float accuracy = await RunSubtitleAsync(fallSpeed, _currentTolerance, token);

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

        _subtitleInstance.Setup(GetSubtitleStartPosition());

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

    private async UniTask<float> RunSubtitleAsync(float fallSpeed, float tolerance, CancellationToken token)
    {
        float missLineY = _targetSlot.anchoredPosition.y - tolerance;

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

    private float EvaluateAttach(float perfectRadius, float tolerance)
    {
        if (null == _subtitleInstance)
        {
            return 0f;
        }

        float distance = Vector2.Distance(_subtitleInstance.RectTransform.anchoredPosition, _targetSlot.anchoredPosition);

        return CalculateAccuracy(distance, perfectRadius, tolerance);
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

    private float GetFallSpeed(int workLevel)
    {
        int levelStep = Mathf.Max(0, workLevel - 1);
        float fallSpeed = _baseFallSpeed + (_fallSpeedPerLevel * levelStep);

        return Mathf.Max(MIN_FALL_SPEED, fallSpeed);
    }

    private float GetTolerance(int workLevel)
    {
        int levelStep = Mathf.Max(0, workLevel - 1);
        float tolerance = _baseTolerance - (_tolerancePerLevel * levelStep);

        return Mathf.Max(_minTolerance, tolerance);
    }

    private float GetPerfectRadius(int workLevel)
    {
        int levelStep = Mathf.Max(0, workLevel - 1);
        float perfectRadius = _basePerfectRadius - (_perfectRadiusPerLevel * levelStep);

        return Mathf.Max(_minPerfectRadius, perfectRadius);
    }

    private void BindInput()
    {
        if (null == _attachButton)
        {
            return;
        }

        _attachButton.BindButtonEvent(OnClickAttach);
    }

    private void UnbindInput()
    {
        if (null == _attachButton)
        {
            return;
        }

        _attachButton.UnBindAllButtonEvent();
    }

    private void OnClickAttach()
    {
        if (_hasAttachResult)
        {
            return;
        }

        _attachAccuracy = EvaluateAttach(_currentPerfectRadius, _currentTolerance);
        _hasAttachResult = true;
    }

    private float GetDeltaTime()
    {
        if (null == GameManager.Instance)
        {
            return Time.deltaTime;
        }

        return GameManager.Time.GameDeltaTime;
    }

    private bool ValidateReferences()
    {
        if (null == _playArea || null == _targetSlot || null == _subtitlePrefab)
        {
            Logger.LogError("참조 비어있음 (PlayArea / TargetSlot / Subtitle)");
            return false;
        }

        if (null == _attachButton)
        {
            Logger.LogWarning("참조 비어있음 (AttachButton)");
        }

        return true;
    }
}
