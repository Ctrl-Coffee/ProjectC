using TMPro;
using UnityEngine;

public class RealHudView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _money;
    [SerializeField] private TextMeshProUGUI _energy;
    [SerializeField] private TextMeshProUGUI _dreamPoint;
    [SerializeField] private TextMeshProUGUI _inspiration;

    [SerializeField] private UIButtonComponent _settingBtn;
    [SerializeField] private UIButtonComponent _goDreamBtn;
    [SerializeField] private UIButtonComponent _goDowntownBtn;

    [SerializeField] private UIButtonComponent _coffeeBtn;
    [SerializeField] private UIButtonComponent _computerBtn;
    [SerializeField] private UIButtonComponent _perkBtn;

    private CurrencyViewModel _currencyViewModel;

    private void OnEnable()
    {
        if(_currencyViewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        _settingBtn.BindButtonEvent(OnOpenSettingUI);

        _goDreamBtn.BindButtonEvent(OnChangeSceenToDream);

        _computerBtn.BindButtonEvent(OnOpenWorkInfoUI);
        _perkBtn.BindButtonEvent(OnOpenPerkInfoUI);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _settingBtn.UnBindButtonAllEvent();
        _goDreamBtn.UnBindButtonAllEvent();

        _coffeeBtn.UnBindButtonAllEvent();
        _computerBtn.UnBindButtonAllEvent();
        _perkBtn.UnBindButtonAllEvent();
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

    protected override void Subscribe()
    {
        _currencyViewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        RefreshAll();
    }

    protected override void UnSubscribe()
    {
        _currencyViewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(CurrencyViewModel.Money):
                _money.text = _currencyViewModel.Money.ToString();
                break;
            case nameof(CurrencyViewModel.Energy):
                _energy.text = _currencyViewModel.Energy.ToString();
                break;
            case nameof(CurrencyViewModel.DreamPoint):
                _dreamPoint.text = _currencyViewModel.DreamPoint.ToString();
                break;
            case nameof(CurrencyViewModel.Inspiration):
                _inspiration.text = _currencyViewModel.Inspiration.ToString();
                break;
        }
    }

    private void OnOpenSettingUI()
    {
        GameManager.UI.OpenSettingUI();
    }

    private void OnChangeSceenToDream()
    {
        GameManager.Instance.ExitReal();
        GameManager.Instance.EnterDream();
    }

    private void OnOpenWorkInfoUI()
    {
        GameManager.UI.OpenWorkInfoUI();
    }

    private void OnOpenPerkInfoUI()
    {
        GameManager.UI.OpenPerkInfoUI();
    }

    private void RefreshAll()
    {
        _money.text = _currencyViewModel.Money.ToString();
        _energy.text = _currencyViewModel.Energy.ToString();
        _dreamPoint.text = _currencyViewModel.DreamPoint.ToString();
        _inspiration.text = _currencyViewModel.Inspiration.ToString();
    }
}
