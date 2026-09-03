using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using System.Threading;
using UnityEngine;

public class CoffeePotViewModel : ViewModelBase<CoffeePotModel>
{
    private const float TICK_INTERVAL_SECONDS = 1f;

    private CancellationTokenSource _tickCts;

    private string _lastRemainText;
    private float _lastChargeProgress;
    private bool _lastIsReady;

    public CoffeePotViewModel(CoffeePotModel model) : base(model)
    {
    }

    public bool IsReady
    {
        get
        {
            return _model.IsReady;
        }
    }

    public float ChargeProgress
    {
        get
        {
            return _model.ChargeProgress;
        }
    }

    public string RemainText
    {
        get
        {
            if (_model.IsReady)
            {
                return string.Empty;
            }

            int totalSeconds = Mathf.CeilToInt(_model.RemainSeconds);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public long TryUse()
    {
        return _model.TryUse();
    }

    public void StartTick()
    {
        StopTick();

        NotifyAll();

        _tickCts = new CancellationTokenSource();

        RunTickLoopAsync(_tickCts.Token).Forget();
    }

    public void StopTick()
    {
        if (null == _tickCts)
        {
            return;
        }

        _tickCts.Cancel();
        _tickCts.Dispose();
        _tickCts = null;
    }

    public override void UnBind()
    {
        StopTick();

        base.UnBind();
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        NotifyAll();
    }

    private async UniTaskVoid RunTickLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(TICK_INTERVAL_SECONDS), ignoreTimeScale: true, cancellationToken: token);

                NotifyChanged();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void NotifyAll()
    {
        _lastChargeProgress = ChargeProgress;
        _lastRemainText = RemainText;
        _lastIsReady = IsReady;

        Raise(nameof(ChargeProgress));
        Raise(nameof(RemainText));
        Raise(nameof(IsReady));
    }

    private void NotifyChanged()
    {
        float chargeProgress = ChargeProgress;

        if (_lastChargeProgress != chargeProgress)
        {
            _lastChargeProgress = chargeProgress;
            Raise(nameof(ChargeProgress));
        }

        string remainText = RemainText;

        if (_lastRemainText != remainText)
        {
            _lastRemainText = remainText;
            Raise(nameof(RemainText));
        }

        bool isReady = IsReady;

        if (_lastIsReady != isReady)
        {
            _lastIsReady = isReady;
            Raise(nameof(IsReady));
        }
    }

    private void Raise(string propertyName)
    {
        base.OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
