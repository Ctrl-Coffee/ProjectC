using System;
using UnityEngine;

public class TimeManagerTemp : MonoBehaviour
{
    public static TimeManagerTemp Instance { get; private set; }

    private readonly CooldownService _cooldownService = new CooldownService();

    private float _currentTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _currentTime = Time.time;

        _cooldownService.UpdateCooldowns(_currentTime);
    }

    public void RequestStartCooldown(string id, float duration, Action onCompleted)
    {
        _cooldownService.StartCooldown(id, _currentTime, duration, onCompleted);
    }

    public void RequestCancelCooldown(string id)
    {
        _cooldownService.CancelCooldown(id);
    }

    public void RequestGetRemainingTime(string id)
    {
        _cooldownService.GetRemainingTime(id, _currentTime);
    }

    public void RequestClearCooldown()
    {
        _cooldownService.ClearCooldown();
    }
}