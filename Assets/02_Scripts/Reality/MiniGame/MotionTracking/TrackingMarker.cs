using UnityEngine;

public class TrackingMarker
{
    private const float MAX_SPEED = 1.4f;

    private float _position = 0.5f;
    private float _velocity = 0f;

    private float _size = 0.07f;
    private float _liftAcceleration = 2.4f;
    private float _gravity = 1.6f;
    private float _bounceFactor = 0.3f;

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

    public void Init(float size, float liftAcceleration, float gravity, float bounceFactor)
    {
        _size = Mathf.Clamp(size, 0.01f, 1f);
        _liftAcceleration = Mathf.Max(0f, liftAcceleration);
        _gravity = Mathf.Max(0f, gravity);
        _bounceFactor = Mathf.Clamp01(bounceFactor);
    }

    public void ResetPosition(float position)
    {
        _position = position;
        _velocity = 0f;

        ClampToTrack();
    }

    public void Move(bool isPressed, float deltaTime)
    {
        if (deltaTime <= 0f)
        {
            return;
        }

        float acceleration = isPressed ? _liftAcceleration : -_gravity;

        _velocity += acceleration * deltaTime;
        _velocity = Mathf.Clamp(_velocity, -MAX_SPEED, MAX_SPEED);

        _position += _velocity * deltaTime;

        ClampToTrack();
    }

    private void ClampToTrack()
    {
        float min = HalfSize;
        float max = 1f - HalfSize;

        if (min >= max)
        {
            _position = 0.5f;
            _velocity = 0f;
            return;
        }

        if (_position < min)
        {
            _position = min;
            _velocity = -_velocity * _bounceFactor;
            return;
        }

        if (_position > max)
        {
            _position = max;
            _velocity = -_velocity * _bounceFactor;
        }
    }
}
