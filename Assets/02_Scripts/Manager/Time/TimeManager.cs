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

    private float _elapsedGameTime;

    private readonly CooldownService _cooldownService = new CooldownService();


#if UNITY_EDITOR
    private TimeSpan _debugTimeOffset = TimeSpan.Zero;
#endif

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

    // 자동업무처럼 앱이 꺼져 있어도 흘러야 하는 시간은 이걸 쓴다.
    // 서버 시각이 붙으면 여기에 보정치를 더한다.
    public DateTime UtcNow
    {
        get
        {
#if UNITY_EDITOR
            return DateTime.UtcNow + _debugTimeOffset;
#else
            return DateTime.UtcNow;
#endif
        }
    }

#if UNITY_EDITOR
    public TimeSpan DebugTimeOffset
    {
        get
        {
            return _debugTimeOffset;
        }
    }

    public void OnUpdate()
    {
        _elapsedGameTime += GameDeltaTime;
        _cooldownService.UpdateCooldowns(_elapsedGameTime);
    }

    public void AddDebugTime(TimeSpan amount)
    {
        _debugTimeOffset += amount;
    }

    public void ResetDebugTime()
    {
        _debugTimeOffset = TimeSpan.Zero;
    }
#endif

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

    public void RequestStartCooldown(string id, float duration, Action onCompleted)
    {
        _cooldownService.StartCooldown(id, _elapsedGameTime, duration, onCompleted);
    }

    public void RequestCancelCooldown(string id)
    {
        _cooldownService.CancelCooldown(id);
    }

    public void RequestGetRemainingTime(string id)
    {
        _cooldownService.GetRemainingTime(id, _elapsedGameTime);
    }

    public void RequestClearCooldown()
    {
        _cooldownService.ClearCooldown();
    }
}
