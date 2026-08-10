using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LabelAttachGameUI : MiniGameBase
{
    private const float MIN_FALL_SPEED = 1f;

    [Header("배치")]
    [SerializeField] private RectTransform _playArea;
    [SerializeField] private RectTransform _targetSlot;
    [SerializeField] private Button _attachButton;

    [SerializeField] private FallingLabel _labelPrefab;

    // TODO: 김경훈 (26.08.10) 라벨 여러개가 되면 PoolManager로 옮기기
    private FallingLabel _labelInstance;

    [Header("모양")]
    [SerializeField] private Sprite[] _shapeSprites;
    [SerializeField] private Image _targetShapeImage;

    [Header("난이도 - 기본값")]
    [SerializeField] private float _baseFallSpeed = 400f;
    [SerializeField] private float _baseTolerance = 150f;

    [Header("난이도 - 업무 레벨당 증감")]
    [SerializeField] private float _fallSpeedPerLevel = 20f;
    [SerializeField] private float _tolerancePerLevel = 5f;
    [SerializeField] private float _minTolerance = 40f;

    [Header("스킵 연출")]
    [SerializeField] private float _skipSpeedMultiplier = 4f;

    private bool _hasAttachResult = false;
    private float _attachAccuracy = 0f;
    private float _currentTolerance = 0f;
    private int _targetShapeId = 0;

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (!ValidateReferences())
        {
            return MiniGameResult.Canceled;
        }

        float fallSpeed = GetFallSpeed(context.WorkLevel);
        _currentTolerance = GetTolerance(context.WorkLevel);

        if (!SetupRound())
        {
            return MiniGameResult.Canceled;
        }

        BindInput();

        try
        {
            float accuracy = await RunLabelAsync(fallSpeed, _currentTolerance, token);

            MiniGameGrade grade = MiniGameGradeTable.GetGrade(accuracy);

            return new MiniGameResult
            {
                IsCompleted = true,
                Completion = MiniGameGradeTable.GetCompletion(grade),
                Grade = grade,
            };
        }
        catch (OperationCanceledException)
        {
            return MiniGameResult.Canceled;
        }
        finally
        {
            UnbindInput();
        }
    }

    public override async UniTask PlaySkipAsync(MiniGameContext context, CancellationToken token)
    {
        if (!ValidateReferences())
        {
            return;
        }

        float fallSpeed = GetFallSpeed(context.WorkLevel) * _skipSpeedMultiplier;

        if (!SetupRound())
        {
            return;
        }

        try
        {
            await RunSkipFallAsync(fallSpeed, token);
            _labelInstance.SnapTo(_targetSlot.anchoredPosition);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private bool SetupRound()
    {
        if (null == EnsureLabel())
        {
            return false;
        }

        _targetShapeId = 0;

        Sprite shapeSprite = GetShapeSprite(_targetShapeId);
        if (null != _targetShapeImage && null != shapeSprite)
        {
            _targetShapeImage.sprite = shapeSprite;
        }

        _labelInstance.Setup(_targetShapeId, shapeSprite, GetLabelStartPosition());

        _hasAttachResult = false;
        _attachAccuracy = 0f;

        return true;
    }

    private FallingLabel EnsureLabel()
    {
        if (null != _labelInstance)
        {
            return _labelInstance;
        }

        if (null == _labelPrefab)
        {
            Logger.LogError("LabelPrefab이 연결되지 않았습니다.");
            return null;
        }

        _labelInstance = Instantiate(_labelPrefab, _playArea, false);

        return _labelInstance;
    }

    private Vector2 GetLabelStartPosition()
    {
        Vector2 startPosition = _targetSlot.anchoredPosition;
        startPosition.y = (_playArea.rect.height * 0.5f) + _labelInstance.RectTransform.rect.height;

        return startPosition;
    }

    private async UniTask<float> RunLabelAsync(float fallSpeed, float tolerance, CancellationToken token)
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
            _labelInstance.Fall(fallSpeed * deltaTime);

            if (_labelInstance.RectTransform.anchoredPosition.y < missLineY)
            {
                return 0f;
            }
        }
    }

    private async UniTask RunSkipFallAsync(float fallSpeed, CancellationToken token)
    {
        float targetY = _targetSlot.anchoredPosition.y;

        while (_labelInstance.RectTransform.anchoredPosition.y > targetY)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            float deltaTime = GetDeltaTime();
            _labelInstance.Fall(fallSpeed * deltaTime);
        }
    }

    private float EvaluateAttach(float tolerance)
    {
        if (null == _labelInstance)
        {
            return 0f;
        }

        if (_labelInstance.ShapeId != _targetShapeId)
        {
            return 0f;
        }

        float distance = Vector2.Distance(_labelInstance.RectTransform.anchoredPosition, _targetSlot.anchoredPosition);

        return CalculateAccuracy(distance, tolerance);
    }

    private float CalculateAccuracy(float distance, float tolerance)
    {
        if (tolerance <= 0f)
        {
            return 0f;
        }

        float accuracy = 1f - (distance / tolerance);

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

    private void BindInput()
    {
        if (null == _attachButton)
        {
            return;
        }

        _attachButton.onClick.AddListener(OnClickAttach);
    }

    private void UnbindInput()
    {
        if (null == _attachButton)
        {
            return;
        }

        _attachButton.onClick.RemoveListener(OnClickAttach);
    }

    private void OnClickAttach()
    {
        if (_hasAttachResult)
        {
            return;
        }

        _attachAccuracy = EvaluateAttach(_currentTolerance);
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

    private Sprite GetShapeSprite(int shapeId)
    {
        if (null == _shapeSprites || _shapeSprites.Length == 0)
        {
            return null;
        }

        if (shapeId < 0 || shapeId >= _shapeSprites.Length)
        {
            return null;
        }

        return _shapeSprites[shapeId];
    }

    private bool ValidateReferences()
    {
        if (null == _playArea || null == _targetSlot || null == _labelPrefab)
        {
            Logger.LogError("참조 비어있음 (PlayArea / TargetSlot / Label)");
            return false;
        }

        if (null == _attachButton)
        {
            Logger.LogWarning("참조 비어있음 (AttachButton)");
        }

        return true;
    }
}
