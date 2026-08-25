using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _bannerNameText;
    [SerializeField] private UIButtonComponent _singleDrawButton;
    [SerializeField] private TextMeshProUGUI _singleCostText;
    [SerializeField] private UIButtonComponent _multiDrawButton;
    [SerializeField] private TextMeshProUGUI _multiCostText;
    [SerializeField] private UIButtonComponent _companionTabButton;
    [SerializeField] private UIButtonComponent _equipmentTabButton;
    [SerializeField] private TextMeshProUGUI _singleDrawLabelText;
    [SerializeField] private TextMeshProUGUI _multiDrawLabelText;
    [SerializeField] private UIButtonComponent _closeButton;

    [Header("배너")]
    [SerializeField] private Image _bannerImage;
    [SerializeField] private Sprite _companionBannerSprite;
    [SerializeField] private Sprite _equipmentBannerSprite;

    [Header("뽑기 연출")]
    [SerializeField] private RectTransform _scrollRect;
    [SerializeField] private Image _flashImage;
    [SerializeField] private int _spinCount = 3;
    [SerializeField] private float _spinDuration = 1f;
    [SerializeField] private float _flashDuration = 0.3f;

    private bool _isDrawing = false;
    private GachaViewModel _viewModel;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        _companionTabButton.BindButtonEvent(OnClickComponionTab);
        _equipmentTabButton.BindButtonEvent(OnClickEquipmentTab);
        _singleDrawButton.BindButtonEvent(OnClickSingleDraw);
        _multiDrawButton.BindButtonEvent(OnClickMultiDraw);
        _closeButton.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _companionTabButton.UnBindButtonAllEvent();
        _equipmentTabButton.UnBindButtonAllEvent();
        _singleDrawButton.UnBindButtonAllEvent();
        _multiDrawButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_viewModel != null )
        {
            _viewModel.UnBind();
            _viewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _viewModel = GameManager.ViewModel.CreateGachaViewModel();
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _viewModel.InitializeModel();
    }

    protected override void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(GachaModel.CurrentType):
            case nameof(CurrencyModel.DreamScroll):
                RefreshAll();
                break;
        }
    }
    private Sprite GetBannerSprite(GachaType gachaType)
    {
        switch (gachaType)
        {
            case GachaType.Companion:
                return _companionBannerSprite;

            case GachaType.Equipment:
                return _equipmentBannerSprite;

            default:
                Logger.LogError($"배너 이미지가 없는 가챠 종류입니다. type : {gachaType}");
                return null;
        }
    }

    private void RefreshAll()
    {
        if (_viewModel == null) return;

        _bannerImage.sprite = GetBannerSprite(_viewModel.CurrentType);
        _bannerNameText.text = _viewModel.BannerName;
        _singleCostText.text = $"필요 재화 : {_viewModel.SingleCost}";
        _multiCostText.text = $"필요 재화 : {_viewModel.MultiCost}";
        _singleDrawButton.SetInteractable(_viewModel.CanDrawSingle);
        _multiDrawButton.SetInteractable(_viewModel.CanDrawMulti);
        _singleDrawLabelText.text = $"{_viewModel.SingleDrawCount}회 뽑기";
        _multiDrawLabelText.text = $"{_viewModel.MultiDrawCount}회 뽑기";
    }

    private void OnClickComponionTab()
    {
        _viewModel.SelectType(GachaType.Companion);
    }

    private void OnClickEquipmentTab()
    {
        _viewModel.SelectType(GachaType.Equipment);
    }

    private void OnClickSingleDraw()
    {
        DrawAsync(_viewModel.SingleDrawCount).Forget();
    }

    private void OnClickMultiDraw()
    {
        DrawAsync(_viewModel.MultiDrawCount).Forget();
    }

    private async UniTaskVoid DrawAsync(int count)
    {
        if (_isDrawing) return;

        IReadOnlyList<GachaResultData> results = _viewModel.Draw(count);

        if (results == null) return;

        _isDrawing = true;

        try
        {
            await PlayDrawEffectAsync(this.GetCancellationTokenOnDestroy());

            GameManager.UI.OpenGachaResultUI(results);
        }
        finally
        {
            _isDrawing = false;
        }
    }

    private async UniTask PlayDrawEffectAsync(CancellationToken token)
    {
        _scrollRect.DOKill();
        _scrollRect.localEulerAngles = Vector3.zero;

        GameManager.Sound.PlaySFX(AddressablePath.Audio.GACHA_SUMMON);
        await _scrollRect.DORotate(new Vector3(0f, 0f, -360f * _spinCount), _spinDuration, RotateMode.FastBeyond360).SetEase(Ease.InCubic).SetUpdate(true).ToUniTask(cancellationToken: token);

        await PlayFlashAsync(token);

        _scrollRect.localEulerAngles = Vector3.zero;
    }

    private async UniTask PlayFlashAsync(CancellationToken token)
    {
        _flashImage.gameObject.SetActive(true);
        _flashImage.color = new Color(1f, 1f, 1f, 0f);

        await _flashImage.DOFade(1f, _flashDuration * 0.3f).SetUpdate(true).ToUniTask(cancellationToken: token);

        await _flashImage.DOFade(0f, _flashDuration * 0.7f).SetUpdate(true).ToUniTask(cancellationToken: token);

        _flashImage.gameObject.SetActive(false);
    }
}
