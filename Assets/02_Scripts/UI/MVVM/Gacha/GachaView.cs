using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _bannerNameText;
    [SerializeField] private Button _singleDrawButton;
    [SerializeField] private TextMeshProUGUI _singleCostText;
    [SerializeField] private Button _multiDrawButton;
    [SerializeField] private TextMeshProUGUI _multiCostText;
    [SerializeField] private Button _companionTabButton;
    [SerializeField] private Button _equipmentTabButton;
    [SerializeField] private TextMeshProUGUI _singleDrawLabelText;
    [SerializeField] private TextMeshProUGUI _multiDrawLabelText;
    [SerializeField] private Button _closeButton;

    private GachaViewModel _viewModel;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        _companionTabButton.onClick.AddListener(OnClickComponionTab);
        _equipmentTabButton.onClick.AddListener(OnClickEquipmentTab);
        _singleDrawButton.onClick.AddListener(OnClickSingleDraw);
        _multiDrawButton.onClick.AddListener(OnClickMultiDraw);
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _companionTabButton.onClick.RemoveListener(OnClickComponionTab);
        _equipmentTabButton.onClick.RemoveListener(OnClickEquipmentTab);
        _singleDrawButton.onClick.RemoveListener(OnClickSingleDraw);
        _multiDrawButton.onClick.RemoveListener(OnClickMultiDraw);
        _closeButton.onClick.RemoveListener(OnClickCloseButton);
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

    private void RefreshAll()
    {
        if (_viewModel == null) return;

        _bannerNameText.text = _viewModel.BannerName;
        _singleCostText.text = $"필요 재화 : {_viewModel.SingleCost}";
        _multiCostText.text = $"필요 재화 : {_viewModel.MultiCost}";
        _singleDrawButton.interactable = _viewModel.CanDrawSingle;
        _multiDrawButton.interactable = _viewModel.CanDrawMulti;
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
        IReadOnlyList<GachaResultData> results = _viewModel.Draw(_viewModel.SingleDrawCount);

        if (results == null) return;

        GameManager.UI.OpenGachaResultUI(results);
    }

    private void OnClickMultiDraw()
    {
        IReadOnlyList<GachaResultData> results = _viewModel.Draw(_viewModel.MultiDrawCount);

        if (results == null) return;

        GameManager.UI.OpenGachaResultUI(results);
    }

   
}
