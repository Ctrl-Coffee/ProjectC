using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

public class MiniGameResultUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _txtGrade;
    [SerializeField] private TextMeshProUGUI _txtAccuracy;
    [SerializeField] private UIButtonComponent _btnClose;

    [Header("연출")]
    [SerializeField] private float _openDuration = 0.25f;
    [SerializeField] private float _closeDuration = 0.2f;

    [Header("재화 표시")]
    [SerializeField] private AwayReportRowUI _rowRewardMoney;
    [SerializeField] private AwayReportRowUI _rowRewardDP;
    [SerializeField] private AwayReportRowUI _rowSpentEnergy;
    [SerializeField] private AwayReportRowUI _rowSpentGold;

    [Header("소설쓰기 전용")]
    [SerializeField] private TextMeshProUGUI _txtNovelSummary;

    private UniTaskCompletionSource _closeRequestedSource;

    public override Tween PlayOpenAnimation()
    {
        transform.localScale = Vector3.zero;

        transform.DOKill();
        return transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public override Tween PlayCloseAnimation()
    {
        transform.DOKill();
        return transform.DOScale(0f, _closeDuration).SetEase(Ease.InBack).SetUpdate(true);
    }

    public override void OnClickCloseButton()
    {
        if (null == _closeRequestedSource)
        {
            return;
        }

        _closeRequestedSource.TrySetResult();
    }

    public void SetResult(MiniGameResult result)
    {
        if (null != _txtGrade)
        {
            _txtGrade.text = result.Grade.ToString();
        }

        if (null != _txtAccuracy)
        {
            _txtAccuracy.text = $"{result.Accuracy:P0}";
        }
        SetRow(_rowRewardMoney, result.RewardMoney);
        SetRow(_rowRewardDP, result.RewardDP);
        SetRow(_rowSpentEnergy, result.SpentEnergy);
        SetRow(_rowSpentGold, result.SpentGold);

        bool isNovel = result.RoundCount > 0;  

        if (null != _txtNovelSummary)
        {
            _txtNovelSummary.gameObject.SetActive(isNovel);

            if (isNovel)
            {
                _txtNovelSummary.text = $"{result.RoundCount}번의 글쓰기, {result.SuccessCount}개의 문장 완성";
            }
        }

        if (null != _txtGrade) _txtGrade.gameObject.SetActive(isNovel == false);
        if (null != _txtAccuracy) _txtAccuracy.gameObject.SetActive(isNovel == false);
    }

    public async UniTask WaitForCloseAsync(CancellationToken token)
    {
        _closeRequestedSource = new UniTaskCompletionSource();

        if (null != _btnClose)
        {
            _btnClose.BindButtonEvent(OnClickCloseButton);
        }
        else
        {
            Logger.LogError("CloseButton이 연결되지 않아 결과창을 닫을 수 없습니다.");
        }

        try
        {
            await _closeRequestedSource.Task.AttachExternalCancellation(token);
        }
        finally
        {
            if (null != _btnClose)
            {
                _btnClose.UnBindButtonAllEvent();
            }

            _closeRequestedSource = null;
            CloseUI();
        }
    }
    private void SetRow(AwayReportRowUI row, long amount)
    {
        if (null == row) return;

        bool isVisible = amount > 0;
        row.gameObject.SetActive(isVisible);

        if (isVisible)
        {
            row.SetAmount(amount);
        }
    }
}
