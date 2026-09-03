using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private UIButtonComponent _heroInfoBtn;

    private GameObject _backgroundInstance;
    private GameObject _heroInventoryBGInstance;
    private GameObject _autoBattleInstance;

    private DreamHudViewModel _dreamViewModel;
    private CurrencyViewModel _currencyViewModel;

    private SwipComponent _swipComponent;

    private void Awake()
    {
        GameObject prefab = 
            GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.DREAM_LOBBY_BACKGROUND);
        _backgroundInstance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        _swipComponent = _backgroundInstance.GetComponentInChildren<SwipComponent>();

        GameObject heroInventoryPrefab = 
            GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.HERO_INVENTORY_BACKGROUND);
        _heroInventoryBGInstance = Instantiate(heroInventoryPrefab, Vector3.zero, Quaternion.identity);
        _heroInventoryBGInstance.SetActive(false);

        GameObject autoBattlePrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.AUTO_BATTLE);

        if (autoBattlePrefab == null)
        {
            Logger.LogError("자동전투 프리팹을 불러오지 못했습니다.");
            return;
        }

        _autoBattleInstance = Instantiate(autoBattlePrefab, Vector3.zero, Quaternion.identity);
        _autoBattleInstance.SetActive(false);
    }

    private void OnEnable()
    {
        if (_currencyViewModel == null || _dreamViewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        _gachaBtn.BindButtonEvent(OnOpenGacha);
        _companionBtn.BindButtonEvent(OnOpenCompanion);
        _stageBtn.BindButtonEvent(OnStage);
        _heroBtn.BindButtonEvent(OnOpenHeroInventory);

        _lobbyBtn.BindButtonEvent(OnChangeSceenToReal);

        _settingBtn.BindButtonEvent(OnOpenSettingUI);
        _heroInfoBtn.BindButtonEvent(OnOpenHeroInfo);

        _backgroundInstance.SetActive(true);
        SetAutoBattleActive(true);
    }

    private void OnDisable()
    {
        UnSubscribe();

        _gachaBtn.UnBindButtonAllEvent();
        _companionBtn.UnBindButtonAllEvent();
        _stageBtn.UnBindButtonAllEvent();
        _heroBtn.UnBindButtonAllEvent();
        _lobbyBtn.UnBindButtonAllEvent();
        _heroInfoBtn.UnBindButtonAllEvent();
        _settingBtn.UnBindButtonAllEvent();

        if(_backgroundInstance != null)
            _backgroundInstance.SetActive(false);

        SetAutoBattleActive(false);
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

    public void SetChapter(int chapter)
    {
        int pageIndex = chapter - 1;
        _swipComponent.SetPage(pageIndex, true);
    }

    protected override void BindViewModel()
    {
        _currencyViewModel = GameManager.ViewModel.CreateCurrencyViewModel();
        _dreamViewModel = GameManager.ViewModel.CreateDreamHudViewModel();
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
        _swipComponent.OnPageChanged += OnPageChanged;

        RefreshAll();
    }

    protected override void UnSubscribe()
    {
        _currencyViewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged; 
        _swipComponent.OnPageChanged -= OnPageChanged;
    }

    private void OnChangeSceenToReal()
    {
        _dreamViewModel.OnChangeSceenToReal();
    }

    private void OnStage()
    {
        if (_dreamViewModel.ExistCurrentContent == false)
            return;

        if(_dreamViewModel.IsOpenInventory)
        {
            HeroInventoryBGOnOff(false);
        }
        
        LobbyBGOnOff(true);
        ShowLobbyButton();

        SetAutoBattleActive(true);
        
        _dreamViewModel.ClearCurrentContent();
    }

    private void OnOpenCompanion()
    {
        HideLobbyButton();
        _dreamViewModel.OnOpenCompanion();
    }


    private void OnOpenHeroInventory()
    {
        HideLobbyButton();
        _dreamViewModel.OnOpenHeroInventory();

        HeroInventoryBGOnOff(true); 
        LobbyBGOnOff(false);

        SetAutoBattleActive(false);
    }

    private void OnOpenGacha()
    {
        _dreamViewModel.OnOpenGacha();
        ShowLobbyButton();
    }

    private void OnOpenSettingUI()
    {
        GameManager.UI.OpenSettingUI();
    }

    private void OnOpenHeroInfo()
    {
        _dreamViewModel.OnOpenHeroInfo();
    }

    private void RefreshAll()
    {
        _dreamPoint.text = _currencyViewModel.DreamPoint.ToString();
        _fragmentDream.text = _currencyViewModel.DreamFragment.ToString();
        _scrollDream.text = _currencyViewModel.DreamScroll.ToString();
        _inspiration.text = _currencyViewModel.Inspiration.ToString();
    }

    private void SetAutoBattleActive(bool isActive)
    {
        if (_autoBattleInstance == null)
            return;

        _autoBattleInstance.SetActive(isActive);
    }

    private void HideLobbyButton()
    {
        _lobbyBtn.gameObject.SetActive(false);
    }

    private void ShowLobbyButton()
    {
        _lobbyBtn.gameObject.SetActive(true);
    }

    private void LobbyBGOnOff(bool isActive)
    {
        if (_backgroundInstance == null)
            return;

        _backgroundInstance.SetActive(isActive);
    }
    private void HeroInventoryBGOnOff(bool isActive)
    {
        if (_heroInventoryBGInstance == null)
            return;

        _heroInventoryBGInstance.SetActive(isActive);
    }

    private void OnPageChanged(int pageIndex)
    {
        int chapter = pageIndex + 1;
        string audioPath = AddressablePath.GetChapterAudioPath(chapter);

        GameManager.Sound.PlayBGM(audioPath);
    }
}
