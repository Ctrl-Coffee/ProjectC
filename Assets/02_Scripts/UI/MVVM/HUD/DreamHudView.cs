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

        _companionBtn.BindButtonEvent(OnOpenCompanion);
        _stageBtn.BindButtonEvent(OnStage);
        _heroBtn.BindButtonEvent(OnOpenHero);
        _lobbyBtn.BindButtonEvent(OnChangeSceenToReal);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _companionBtn.UnBindButtonAllEvent();
        _stageBtn.UnBindButtonAllEvent();
        _heroBtn.UnBindButtonAllEvent();
        _lobbyBtn.UnBindButtonAllEvent();
    }

    protected override void BindViewModel()
    {
        _currencyViewModel = GameManager.ViewModel.CreateCurrencyViewModel();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch(propertyName)
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

        _currentContent.CloseUI();
        _currentContent = null;
    }

    private async void OnOpenCompanion()
    {
        var content = await GameManager.UI.OpenCompanionInventory();

        CloseCurrentContent(content);
    }


    private async void OnOpenHero()
    {
        var content = await GameManager.UI.OpenHeroInventory();

        CloseCurrentContent(content);
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
}
