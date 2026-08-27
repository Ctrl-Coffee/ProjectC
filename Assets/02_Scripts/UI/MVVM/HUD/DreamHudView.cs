using TMPro;
using UnityEngine;

public class DreamHudView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _dreamPoint;
    [SerializeField] private TextMeshProUGUI _fragmentDream;
    [SerializeField] private TextMeshProUGUI _scrollDream;
    [SerializeField] private TextMeshProUGUI _inspiration;

    [SerializeField] private UIButtonComponent _settingBtn;

    [SerializeField] private UIButtonComponent _gachaBtn;
    [SerializeField] private UIButtonComponent _companionBtn;
    [SerializeField] private UIButtonComponent _stageBtn;
    [SerializeField] private UIButtonComponent _heroBtn;

    [SerializeField] private UIButtonComponent _lobbyBtn;

    private UIBase _currentContent;

    private CurrencyViewModel _currencyViewModel;

    private void OnEnable()
    {
        if (_currencyViewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        _gachaBtn.BindButtonEvent(OnOpenGacha);
        _companionBtn.BindButtonEvent(OnOpenCompanion);
        _stageBtn.BindButtonEvent(OnStage);
        _heroBtn.BindButtonEvent(OnOpenHero);

        _lobbyBtn.BindButtonEvent(OnChangeSceenToReal);

        _settingBtn.BindButtonEvent(OnOpenSettingUI);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _gachaBtn.UnBindButtonAllEvent();
        _companionBtn.UnBindButtonAllEvent();
        _stageBtn.UnBindButtonAllEvent();
        _heroBtn.UnBindButtonAllEvent();
        _lobbyBtn.UnBindButtonAllEvent();
        _settingBtn.UnBindButtonAllEvent();
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_currencyViewModel != null)
        {
            _currencyViewModel.UnBind();
            _currencyViewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _currencyViewModel = GameManager.ViewModel.CreateCurrencyViewModel();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(CurrencyViewModel.DreamPoint):
                _dreamPoint.text = _currencyViewModel.DreamPoint.ToString();
                break;
            case nameof(CurrencyViewModel.DreamFragment):
                _fragmentDream.text = _currencyViewModel.DreamFragment.ToString();
                break;
            case nameof(CurrencyViewModel.DreamScroll):
                _scrollDream.text = _currencyViewModel.DreamScroll.ToString();
                break;
            case nameof(CurrencyViewModel.Inspiration):
                _inspiration.text = _currencyViewModel.Inspiration.ToString();
                break;
        }
    }

    protected override void Subscribe()
    {
        _currencyViewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        RefreshAll();
    }

    protected override void UnSubscribe()
    {
        _currencyViewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
    }

    private void OnChangeSceenToReal()
    {
        GameManager.Instance.ExitDream();
        GameManager.Instance.EnterReal();
    }

    private void OnStage()
    {
        if (_currentContent == null)
            return;

        ShowLobbyButton();
        _currentContent.CloseUI();
        _currentContent = null;
    }

    private async void OnOpenCompanion()
    {
        var content = await GameManager.UI.OpenCompanionInventory();
        HideLobbyButton();
        CloseCurrentContent(content);
    }


    private async void OnOpenHero()
    {
        var content = await GameManager.UI.OpenHeroInventory();
        HideLobbyButton();
        CloseCurrentContent(content);
    }

    private async void OnOpenGacha()
    {
        var content = await GameManager.UI.OpenGachaView();

        ShowLobbyButton();
        CloseCurrentContent(content);
    }

    private void OnOpenSettingUI()
    {
        GameManager.UI.OpenSettingUI();
    }

    private void CloseCurrentContent(UIBase content)
    {
        if (content == null)
            return;

        if (_currentContent != null && _currentContent != content)
        {
            _currentContent.CloseUI();
        }

        _currentContent = content;
    }

    private void RefreshAll()
    {
        _dreamPoint.text = _currencyViewModel.DreamPoint.ToString();
        _fragmentDream.text = _currencyViewModel.DreamFragment.ToString();
        _scrollDream.text = _currencyViewModel.DreamScroll.ToString();
        _inspiration.text = _currencyViewModel.Inspiration.ToString();
    }

    private void HideLobbyButton()
    {
        _lobbyBtn.gameObject.SetActive(false);
    }

    private void ShowLobbyButton()
    {
        _lobbyBtn.gameObject.SetActive(true);
    }
}
