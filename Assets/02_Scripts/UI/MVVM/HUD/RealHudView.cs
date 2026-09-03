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

    [SerializeField] private UIButtonComponent _computerBtn;
    [SerializeField] private UIButtonComponent _perkBtn;

    [SerializeField] private CoffeePotView _coffeePotView;

    private GameObject _backgroundInstance;

    private CurrencyViewModel _currencyViewModel;

    private void Awake()
    {
        GameObject prefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.REAL_LOBBY_BACKGROUND);
        _backgroundInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }

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

        _backgroundInstance.SetActive(true);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _settingBtn.UnBindButtonAllEvent();
        _goDreamBtn.UnBindButtonAllEvent();

        _computerBtn.UnBindButtonAllEvent();
        _perkBtn.UnBindButtonAllEvent();

        if (_backgroundInstance != null)
            _backgroundInstance.SetActive(false);
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

    public void OnChangeSceenToDream()
    {
        GameManager.Instance.ExitReal();
        GameManager.Instance.EnterDream();
    }

    public void OnOpenWorkInfoUI()
    {
        GameManager.UI.OpenWorkInfoUI();
    }

    // 커피포트 오브젝트를 눌렀을 때 InteractObjectHandler 가 부른다.
    public void OnCoffeePot()
    {
        if (null == _coffeePotView)
        {
            return;
        }

        _coffeePotView.OnClickCoffeePot();
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
