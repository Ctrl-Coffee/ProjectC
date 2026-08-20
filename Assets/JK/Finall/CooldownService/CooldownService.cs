using System;
using System.Collections.Generic;
using UnityEngine;

public class CooldownService
{
    private const float COOLDOWN_TICK_INTERVAL = 0.05f;

    private float _lastTickTime;

    private readonly Dictionary<string, float> _cooldownEndTimes = new Dictionary<string, float>();
    private readonly Dictionary<string, Action> _cooldownCallbacks = new Dictionary<string, Action>();

    private readonly List<string> _finishedIds = new List<string>();

    public void UpdateCooldowns(float currentTime)
    {
        if (_cooldownEndTimes.Count <= 0)
        {
            return;
        }

        if (currentTime - _lastTickTime < COOLDOWN_TICK_INTERVAL)
        {
            return;
        }

        _lastTickTime = currentTime;

        foreach (KeyValuePair<string, float> skillCooldown in _cooldownEndTimes)
        {
            float remainingTime = skillCooldown.Value - currentTime;

            if (remainingTime <= 0f)
            {
                _finishedIds.Add(skillCooldown.Key);
            }
        }

        if (_finishedIds.Count <= 0)
        {
            return;
        }

        foreach (string id in _finishedIds)
        {
            _cooldownEndTimes.Remove(id);

            if (_cooldownCallbacks.TryGetValue(id, out Action callback))
            {
                _cooldownCallbacks.Remove(id);
                callback?.Invoke();
            }
        }

        _finishedIds.Clear();
    }

    public void StartCooldown(string id, float duration, float currentTime, Action onCompleted)
    {
        if (duration < 0f)
        {
            Debug.LogError($"'{duration}' 유효하지 않은 쿨타임 값입니다.");
            return;
        }

        _cooldownEndTimes[id] = currentTime + duration;
        _cooldownCallbacks[id] = onCompleted;
    }

    public float GetRemainingTime(string id, float currentTime)
    {
        if (!_cooldownEndTimes.TryGetValue(id, out float endTime))
        {
            return 0f;
        }

        return Mathf.Max(0f, endTime - currentTime);
    }

    public void CancelCooldown(string id)
    {
        _cooldownEndTimes.Remove(id);
        _cooldownCallbacks.Remove(id);
    }

    public void ClearCooldown()
    {
        _cooldownEndTimes.Clear();
        _cooldownCallbacks.Clear();
    }
}