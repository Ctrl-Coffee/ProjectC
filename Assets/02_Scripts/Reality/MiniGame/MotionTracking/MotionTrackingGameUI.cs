using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MotionTrackingGameUI : MiniGameBase
{
    private const float START_POSITION = 0.5f;

    [Header("배치")]
    [SerializeField] private RectTransform _trackRect;
    [SerializeField] private RectTransform _targetRect;
    [SerializeField] private RectTransform _markerRect;
    [SerializeField] private Image _imgProgress;
    [SerializeField] private TextMeshProUGUI _txtTimer;
    [SerializeField] private UIHoldButtonComponent _holdArea;

    [Header("마커 - 조작감")]
    [SerializeField] private float _liftAcceleration = 2.4f;
    [SerializeField] private float _gravity = 1.6f;
    [SerializeField] private float _bounceFactor = 0.3f;

    [Header("타겟 - 이동")]
    [SerializeField] private float _targetSize = 0.26f;
    [SerializeField] private float _targetSpeed = 0.55f;
    [SerializeField] private float _targetSmoothTime = 0.35f;
    [SerializeField] private float _targetJumpRange = 0.45f;
    [SerializeField] private Vector2 _targetHoldTimeRange = new Vector2(0.35f, 1.1f);

    [Header("게이지")]
    [SerializeField] private float _playDurationSeconds = 10f;
    [SerializeField] private float _fillRate = 0.16f;
    [SerializeField] private float _drainRate = 0.08f;

    [Header("게이지 사운드")]
    [SerializeField] private float _gaugeMinPitch = 0.8f;
    [SerializeField] private float _gaugeMaxPitch = 1.6f;

    private TrackingMarker _marker = new();
    private TrackingTarget _target = new();

    private GaugeSoundLoop _gaugeSound = new();

    private float _progress = 0f;
    private float _elapsedTime = 0f;

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (!ValidateReferences())
        {
            return MiniGameResult.Canceled;
        }

        SetupRound();

        try
        {
            float accuracy = await RunTrackingAsync(token);

            return MiniGameResult.Completed(accuracy);
        }
        finally
        {
            _gaugeSound.Stop();
        }
    }

    protected override void ClearGame()
    {
        if (!HasViewReferences())
        {
            return;
        }

        ResetPlayState();
        RefreshView();

        _gaugeSound.Stop();
    }

    private void SetupRound()
    {
        _marker.Init(GetMarkerSize(), _liftAcceleration, _gravity, _bounceFactor);
        _target.Init(_targetSize, _targetSpeed, _targetSmoothTime, _targetJumpRange, _targetHoldTimeRange);

        ResetPlayState();

        SetTargetSize();
        RefreshView();

        _gaugeSound.Init(gameObject, _gaugeMinPitch, _gaugeMaxPitch);
        _gaugeSound.Play();
    }

    private void ResetPlayState()
    {
        _marker.ResetPosition(START_POSITION);
        _target.ResetPosition(START_POSITION);

        _progress = 0f;
        _elapsedTime = 0f;

        if (null != _holdArea)
        {
            _holdArea.ResetButtonPress();
        }
    }

    private async UniTask<float> RunTrackingAsync(CancellationToken token)
    {
        while (_elapsedTime < _playDurationSeconds)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, token);

            float deltaTime = GetDeltaTime();

            if (deltaTime <= 0f)
            {
                continue;
            }

            deltaTime = Mathf.Min(deltaTime, _playDurationSeconds - _elapsedTime);
            _elapsedTime += deltaTime;

            _marker.Move(IsPressed(), deltaTime);
            _target.Move(deltaTime);

            ApplyProgress(_target.IsTracking(_marker.Position), deltaTime);
            RefreshView();
        }

        return _progress;
    }

    private void ApplyProgress(bool isTracking, float deltaTime)
    {
        if (isTracking)
        {
            _progress += _fillRate * deltaTime;
        }
        else
        {
            _progress -= _drainRate * deltaTime;
        }

        _progress = Mathf.Clamp01(_progress);
    }

    private void RefreshView()
    {
        float trackHeight = _trackRect.rect.height;

        SetAnchoredY(_markerRect, (_marker.Position - 0.5f) * trackHeight);
        SetAnchoredY(_targetRect, (_target.Position - 0.5f) * trackHeight);

        if (null != _imgProgress)
        {
            _imgProgress.fillAmount = _progress;
        }

        if (null != _txtTimer)
        {
            _txtTimer.text = GetRemainingSeconds().ToString("F1");
        }

        _gaugeSound.SetProgress(_progress);
    }

    private float GetRemainingSeconds()
    {
        return Mathf.Max(0f, _playDurationSeconds - _elapsedTime);
    }

    private void SetTargetSize()
    {
        Vector2 size = _targetRect.sizeDelta;
        size.y = _trackRect.rect.height * _targetSize;

        _targetRect.sizeDelta = size;
    }

    private void SetAnchoredY(RectTransform target, float y)
    {
        Vector2 position = target.anchoredPosition;
        position.y = y;

        target.anchoredPosition = position;
    }

    private float GetMarkerSize()
    {
        return _markerRect.rect.height / _trackRect.rect.height;
    }

    private bool IsPressed()
    {
        if (null == _holdArea)
        {
            return false;
        }

        return _holdArea.IsPressed;
    }

    private bool HasViewReferences()
    {
        if (null == _trackRect || null == _targetRect || null == _markerRect)
        {
            return false;
        }

        return _trackRect.rect.height > 0f;
    }

    private bool ValidateReferences()
    {
        if (null == _trackRect || null == _targetRect || null == _markerRect)
        {
            Logger.LogError("참조 비어있음 (TrackRect / TargetRect / MarkerRect)");
            return false;
        }

        if (_trackRect.rect.height <= 0f)
        {
            Logger.LogError("TrackRect의 높이가 0입니다.");
            return false;
        }

        if (null == _holdArea)
        {
            Logger.LogError("참조 비어있음 (HoldArea) - 조작이 불가능합니다.");
            return false;
        }

        if (null == _imgProgress)
        {
            Logger.LogWarning("참조 비어있음 (ProgressImage)");
        }

        if (null == _txtTimer)
        {
            Logger.LogWarning("참조 비어있음 (TimerText)");
        }

        return true;
    }
}
