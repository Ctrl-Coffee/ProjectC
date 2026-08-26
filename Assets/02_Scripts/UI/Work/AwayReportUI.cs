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

    [SerializeField] private AwayReportRowUI _rowEnergy;
    [SerializeField] private AwayReportRowUI _rowMoney;
    [SerializeField] private AwayReportRowUI _rowDreamPoint;

    [SerializeField] private UIButtonComponent _btnConfirm;

    private Sequence _countUpSequence;
    private Tween _openTween;

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

        if (null == _btnConfirm)
        {
            return;
        }

        _btnConfirm.UnBindButtonAllEvent();
    }

    public void SetReport(AwayReport report)
    {
        SetAwayDuration(report.AwayDuration);

        PlayCountUp(report);
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

    private void PlayCountUp(AwayReport report)
    {
        KillCountUp();

        _countUpSequence = DOTween.Sequence().SetUpdate(true);

        float at = GetOpenRemainSeconds() + COUNT_UP_OPEN_GAP;

        at = InsertRow(_rowEnergy, report.Energy, at);
        at = InsertRow(_rowMoney, report.Money, at);
        at = InsertRow(_rowDreamPoint, report.DreamPoint, at);

        _countUpSequence.OnKill(OnCountUpKilled);
    }

    private float InsertRow(AwayReportRowUI row, long amount, float atPosition)
    {
        if (null == row)
        {
            Logger.LogError("자리비움 리포트 행이 연결되지 않았습니다.");
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
