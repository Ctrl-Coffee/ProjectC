using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoBattleRewardUI : UIBase
{
    private const float COUNT_UP_STEP_DELAY = 0.12f;
    private const float COUNT_UP_OPEN_GAP = 0.08f;

    [SerializeField] private AwayReportRowUI[] _rows;
    [SerializeField] private UIButtonComponent _btnConfirm;

    private Sequence _countUpSequence;
    private Tween _openTween;

    private AutoBattlePendingReward _reward;
    private Action _onPayout;

    private bool _isCloseRequested;

    public override Tween PlayOpenAnimation()
    {
        _openTween = base.PlayOpenAnimation();
        return _openTween;
    }

    private void OnEnable()
    {
        if (null == _btnConfirm)
        {
            Logger.LogError("확인 버튼이 연결되지 않아 정산 팝업을 닫을 수 없습니다.");
            return;
        }

        _btnConfirm.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        KillCountUp();

        _openTween = null;

        if (!_isCloseRequested)
        {
            Payout();
        }

        if (null == _btnConfirm)
        {
            return;
        }

        _btnConfirm.UnBindButtonAllEvent();
    }

    public void SetReward(AutoBattlePendingReward reward, Action onPayout)
    {
        _reward = reward;
        _onPayout = onPayout;
        _isCloseRequested = false;

        GameManager.Sound.PlaySFX(AddressablePath.Audio.AWAY_REWARD);

        PlayCountUp();
    }

    public override void OnClickCloseButton()
    {
        if (_isCloseRequested)
        {
            return;
        }

        _isCloseRequested = true;

        KillCountUp();

        PlayFlyEffect();

        base.OnClickCloseButton();
    }

    private void PlayFlyEffect()
    {
        List<ICurrencyEffectSource> sources = CollectVisibleRows();

        if (0 == sources.Count)
        {
            Payout();
            return;
        }

        CurrencyFlyEffect effect = CurrencyFlyEffect.GetOrCreate();

        if (null == effect)
        {
            Payout();
            return;
        }

        effect.Play(sources, null, Payout);
    }

    private List<ICurrencyEffectSource> CollectVisibleRows()
    {
        List<ICurrencyEffectSource> sources = new List<ICurrencyEffectSource>();

        for (int i = 0; null != _rows && i < _rows.Length; i++)
        {
            if (null == _rows[i] || false == _rows[i].IsVisible)
            {
                continue;
            }

            sources.Add(_rows[i]);
        }

        return sources;
    }

    private void Payout()
    {
        Action callback = _onPayout;
        _onPayout = null;

        if (null == callback)
        {
            return;
        }

        callback();
    }

    private void PlayCountUp()
    {
        KillCountUp();

        if (null == _rows)
        {
            Logger.LogError("정산 팝업의 재화 행이 연결되지 않았습니다.");
            return;
        }

        _countUpSequence = DOTween.Sequence().SetUpdate(true);

        float at = GetOpenRemainSeconds() + COUNT_UP_OPEN_GAP;

        for (int i = 0; i < _rows.Length; i++)
        {
            at = InsertRow(_rows[i], at);
        }

        _countUpSequence.OnKill(OnCountUpKilled);
    }

    private float InsertRow(AwayReportRowUI row, float atPosition)
    {
        if (null == row)
        {
            Logger.LogError("정산 팝업의 재화 행이 연결되지 않았습니다.");
            return atPosition;
        }

        long amount = null == _reward ? 0 : _reward.GetAmount(row.CurrencyType);

        row.gameObject.SetActive(0 < amount);

        if (amount <= 0)
        {
            return atPosition;
        }

        row.SetAmount(amount);

        _countUpSequence.Insert(atPosition, row.CreateCountUpSequence(amount));

        return atPosition + COUNT_UP_STEP_DELAY;
    }

    private float GetOpenRemainSeconds()
    {
        if (null == _openTween || !_openTween.IsActive())
        {
            return 0f;
        }

        float remain = _openTween.Duration() - _openTween.Elapsed();

        return remain > 0f ? remain : 0f;
    }

    private void KillCountUp()
    {
        if (null == _countUpSequence)
        {
            return;
        }

        Sequence sequence = _countUpSequence;
        _countUpSequence = null;

        sequence.Kill(complete: true);
    }

    private void OnCountUpKilled()
    {
        _countUpSequence = null;
    }
}
