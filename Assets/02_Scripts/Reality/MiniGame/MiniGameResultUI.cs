using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameResultUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _txtGrade;
    [SerializeField] private TextMeshProUGUI _txtAccuracy;
    [SerializeField] private UIButtonComponent _btnClose;
    [SerializeField] private TextMeshProUGUI[] _extraFailStamps;
    [SerializeField] private float[] _extraStampAngles = { 8f, -25f };
    [SerializeField] private float _multiStampInterval = 0.25f;

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

    [Header("주사위 스탬프")]
    [SerializeField] private TextMeshProUGUI _txtStamp;
    [SerializeField] private float _stampStartScale = 3f;
    [SerializeField] private float _stampDuration = 0.25f;
    [SerializeField] private float _stampAngle = -12f;
    [SerializeField] private Color _stampSuccessColor = new Color(0.2f, 0.7f, 0.3f);
    [SerializeField] private Color _stampFailColor = new Color(0.8f, 0.2f, 0.2f);

    [Header("대성공 폭죽")]
    [SerializeField] private Image _sparkTemplate;
    [SerializeField] private int _sparkCount = 12;
    [SerializeField] private float _sparkDistance = 250f;
    [SerializeField] private float _sparkDuration = 0.6f;

    private bool _isStampSuccess;
    private UniTaskCompletionSource _closeRequestedSource;
    private List<Image> _sparks = new();

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
        
        SetRow(_rowRewardMoney, result.RewardMoney, false);
        SetRow(_rowRewardDP, result.RewardDP, false);
        SetRow(_rowSpentEnergy, result.SpentEnergy, true);
        SetRow(_rowSpentGold, result.SpentGold, true);

        switch (result.GameType)
        {
            case MiniGameType.NovelWriting:
                SetTextActive(_txtGrade, false);
                SetTextActive(_txtAccuracy, false);
                SetTextActive(_txtNovelSummary, true);
                SetTextActive(_txtStamp, false);
                HideExtraStamps();
                HideSpark();
                if (_txtNovelSummary != null)
                {
                    _txtNovelSummary.text = $"{result.RoundCount}번의 글쓰기, {result.SuccessCount}개의 문장 완성";
                }
                break;

            case MiniGameType.DiceGamble:
                SetTextActive(_txtGrade, false);
                SetTextActive(_txtAccuracy, false);
                SetTextActive(_txtNovelSummary, false);
                HideExtraStamps();
                PlayStampEffect(result);

                if (result.IsCriticalSuccess)
                {
                    PlayFireworkEffect();

                }

                break;

            default:
                SetTextActive(_txtGrade, true);
                SetTextActive(_txtAccuracy, true);
                SetTextActive(_txtNovelSummary, false);
                SetTextActive(_txtStamp, false);
                HideExtraStamps();
                HideSpark();

                if (null != _txtGrade)
                {
                    _txtGrade.text = result.Grade.ToString();
                }

                if (null != _txtAccuracy)
                {
                    _txtAccuracy.text = $"{result.Accuracy:P0}";
                }
                break;
        }
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
            PlayRewardFlyEffect();
            CloseUI();
        }
    }
    private void SetRow(AwayReportRowUI row, long amount, bool isSpent)
    {
        if (null == row) return;

        bool isVisible = amount > 0;
        row.gameObject.SetActive(isVisible);

        if (isVisible)
        {
            row.SetAmount(amount, isSpent);
        }
    }
    private void PlayRewardFlyEffect()
    {
        List<ICurrencyEffectSource> sources = new List<ICurrencyEffectSource>();

        if (null != _rowRewardMoney && _rowRewardMoney.IsVisible)
        {
            sources.Add(_rowRewardMoney);
        }

        if (null != _rowRewardDP && _rowRewardDP.IsVisible)
        {
            sources.Add(_rowRewardDP);
        }

        if (sources.Count == 0) return;

        CurrencyFlyEffect.GetOrCreate().Play(sources, null, null);
    }

    private void SetTextActive(TextMeshProUGUI text, bool isActive)
    {
        if (null == text) return;

        text.gameObject.SetActive(isActive);
    }

    private void PlayStampEffect(MiniGameResult result)
    {
        PlayOneStamp(_txtStamp, result.IsSuccess, _stampAngle, 0f);

        if (result.IsCriticalFail)
        {
            for (int i =0; i < _extraFailStamps.Length;  i++)
            {
                PlayOneStamp(_extraFailStamps[i], false, _extraStampAngles[i], (i + 1) * _multiStampInterval);
            }
        }
    }

    private void PlayStampSound()
    {
        GameManager.Sound.PlaySFX(_isStampSuccess ? AddressablePath.Audio.STAMP_SUCCESS : AddressablePath.Audio.STAMP_FAIL);
    }

    private void PlayOneStamp(TextMeshProUGUI stamp, bool isSuccess, float angle, float delay)
    {
        if (stamp == null) return;

        _isStampSuccess = isSuccess;
        stamp.gameObject.SetActive(true);


        stamp.text = isSuccess ? "[SUCCESS]" : "[FAILED]";
        stamp.color = isSuccess ? _stampSuccessColor : _stampFailColor;
        stamp.enabled = true;

        RectTransform stampRect = stamp.rectTransform;
        stampRect.DOKill();
        stampRect.localScale = Vector3.one * _stampStartScale;
        stampRect.localEulerAngles = new Vector3(0f, 0f, angle);

        stampRect.DOScale(1f, _stampDuration).SetEase(Ease.InQuad).SetUpdate(true).SetDelay(_openDuration + 0.1f + delay).OnComplete(PlayStampSound);
    }

    private void HideExtraStamps()
    {
        for (int i = 0; i < _extraFailStamps.Length; i++)
        {
            SetTextActive(_extraFailStamps[i], false);
        }
    }

    private Image GetSpark()
    {
        foreach (Image spark in _sparks)
        {
            if (spark.gameObject.activeSelf == false)
            {
                return spark;
            }
        }

        Image newInstance = Instantiate(_sparkTemplate, _sparkTemplate.transform.parent);
        _sparks.Add(newInstance);

        return newInstance;
    }

    private void PlayFireworkEffect()
    {
        Vector2 startPosition = _sparkTemplate.rectTransform.anchoredPosition;
        float startDelay = _openDuration + 0.1f + _stampDuration;

        for (int i = 0; i < _sparkCount; i++)
        {
            Image spark = GetSpark();

            float angle = i * (360f / _sparkCount) * Mathf.Deg2Rad;
            Vector2 target = startPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _sparkDistance;

            RectTransform sparkRect = spark.rectTransform;
            sparkRect.DOKill();
            spark.DOKill();
            spark.gameObject.SetActive(true);
            sparkRect.anchoredPosition = startPosition;
            spark.color = _sparkTemplate.color;
            spark.DOFade(0f, _sparkDuration).SetUpdate(true).SetDelay(startDelay);
            sparkRect.DOAnchorPos(target, _sparkDuration).SetEase(Ease.OutQuad).SetUpdate(true).SetDelay(startDelay);
        }
    }

    private void HideSpark()
    {
        for (int i = 0; i < _sparks.Count; i++)
        {
            _sparks[i].gameObject.SetActive(false);
        }
    }
}
