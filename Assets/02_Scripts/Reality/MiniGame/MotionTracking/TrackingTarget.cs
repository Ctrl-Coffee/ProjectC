using UnityEngine;

public class TrackingTarget
{
    private float _position = 0.5f;
    private float _targetPosition = 0.5f;
    private float _velocity = 0f;
    private float _remainingHoldTime = 0f;

    private float _size = 0.26f;
    private float _maxSpeed = 0.55f;
    private float _smoothTime = 0.35f;
    private float _jumpRange = 0.45f;
    private float _minHoldTime = 0.35f;
    private float _maxHoldTime = 1.1f;

    public float Position
    {
        get
        {
            return _position;
        }
    }

    private float HalfSize
    {
        get
        {
            return _size * 0.5f;
        }
    }

    private float MinPosition
    {
        get
        {
            return HalfSize;
        }
    }

    private float MaxPosition
    {
        get
        {
            return 1f - HalfSize;
        }
    }

    public void Init(float size, float maxSpeed, float smoothTime, float jumpRange, Vector2 holdTimeRange)
    {
        _size = Mathf.Clamp(size, 0.01f, 1f);
        _maxSpeed = Mathf.Max(0.01f, maxSpeed);
        _smoothTime = Mathf.Max(0.01f, smoothTime);
        _jumpRange = Mathf.Clamp(jumpRange, 0.01f, 1f);
        _minHoldTime = Mathf.Max(0.01f, holdTimeRange.x);
        _maxHoldTime = Mathf.Max(_minHoldTime, holdTimeRange.y);
    }

    public void ResetPosition(float position)
    {
        _position = ClampToTrack(position);
        _targetPosition = _position;
        _velocity = 0f;
        _remainingHoldTime = 0f;
    }

    public void Move(float deltaTime)
    {
        if (deltaTime <= 0f)
        {
            return;
        }

        _remainingHoldTime -= deltaTime;

        if (_remainingHoldTime <= 0f)
        {
            PickNextTarget();
        }

        _position = Mathf.SmoothDamp(_position, _targetPosition, ref _velocity, _smoothTime, _maxSpeed, deltaTime);
        _position = ClampToTrack(_position);
    }

    public bool IsTracking(float markerPosition)
    {
        return Mathf.Abs(markerPosition - _position) <= HalfSize;
    }

    private void PickNextTarget()
    {
        float min = Mathf.Max(MinPosition, _position - _jumpRange);
        float max = Mathf.Min(MaxPosition, _position + _jumpRange);

        if (min >= max)
        {
            _targetPosition = 0.5f;
        }
        else
        {
            _targetPosition = Random.Range(min, max);
        }

        _remainingHoldTime = Random.Range(_minHoldTime, _maxHoldTime);
    }

    private float ClampToTrack(float position)
    {
        if (MinPosition >= MaxPosition)
        {
            return 0.5f;
        }

        return Mathf.Clamp(position, MinPosition, MaxPosition);
    }
}
