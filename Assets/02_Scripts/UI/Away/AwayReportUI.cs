using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class AwayReportUI : UIBase
{
    private const float COUNT_UP_STEP_DELAY = 0.12f;

    // 팝업이 다 열린 뒤 한 박자 쉬고 열기
    private const float COUNT_UP_OPEN_GAP = 0.08f;

    [SerializeField] private TextMeshProUGUI _txtAwayDuration;
    [SerializeField] private AwayReportRowUI[] _rows;
    [SerializeField] private UIButtonComponent _btnConfirm;

    private Sequence _countUpSequence;
    private Tween _openTween;

    private AwayReport _report;

    private bool _hasReport;
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
            Logger.LogError("확인 버튼이 연결되지 않아 리포트를 닫을 수 없습니다.");
            return;
        }

        _btnConfirm.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        KillCountUp();

        _openTween = null;

        if (_hasReport && !_isCloseRequested)
        {
            AwayReportFlow.OnReportClosed(null);
        }

        _hasReport = false;

        if (null == _btnConfirm)
        {
            return;
        }

        _btnConfirm.UnBindButtonAllEvent();
    }

    public void SetReport(AwayReport report)
    {
        _report = report;
        _hasReport = true;
        _isCloseRequested = false;

        AwayReportFlow.OnReportOpened();

        SetAwayDuration(report.AwayDuration);

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

        AwayReportFlow.OnReportClosed(_rows);

        base.OnClickCloseButton();
    }

    private void SetAwayDuration(TimeSpan duration)
    {
        if (null == _txtAwayDuration)
        {
            Logger.LogError("자리비움 시간 텍스트가 연결되지 않았습니다.");
            return;
        }

        _txtAwayDuration.text = Utils.FormatDuration((float)duration.TotalSeconds);
    }

    private void PlayCountUp()
    {
        KillCountUp();

        if (null == _rows)
        {
            Logger.LogError("자리비움 리포트 행이 연결되지 않았습니다.");
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
            Logger.LogError("자리비움 리포트 행이 연결되지 않았습니다.");
            return atPosition;
        }

        long amount = _report.GetAmount(row.CurrencyType);

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
