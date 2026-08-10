using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class TimeManager
{
    public event Action OnPaused;
    public event Action OnResumed;

    private float _timeRate = 1f;
    private int _pauseCount = 0;

    public float GameDeltaTime
    {
        get
        {
            return IsPaused ? 0f : Time.deltaTime * _timeRate;
        }
    }

    public bool IsPaused
    {
        get
        {
            return _pauseCount > 0;
        }
    }

    public void Init()
    {
        _pauseCount = 0;
    }

    public void Pause()
    {
        _pauseCount++;

        if (_pauseCount == 1)
        {
            OnPaused?.Invoke();
        }
    }

    public void Resume()
    {
        if (_pauseCount == 0)
            return;

        _pauseCount = Mathf.Max(0, _pauseCount - 1);

        if (_pauseCount == 0)
        {
            OnResumed?.Invoke();
        }
    }

    public void ChangeTimeScale(float rate)
    {
        _timeRate = rate;
    }

    public void ResetTimeScale()
    {
        _timeRate = 1f;
    }

    public async UniTask<bool> WaitForGameSeconds(float duration, CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
        {
            return false;
        }

        float remainingTime = duration;

        while (remainingTime > 0f)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);

            if (token.IsCancellationRequested)
            {
                return false;
            }

            remainingTime -= GameDeltaTime;
        }

        return true;
    }
}
