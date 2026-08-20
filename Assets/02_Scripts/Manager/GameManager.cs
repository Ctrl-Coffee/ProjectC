using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public static ResourceManager Resource { get { return Instance._resourceManager; } }
    public static NetworkManager Network { get { return Instance._networkManager; } }
    public static DataTableManager DataTable { get { return Instance._dataTable; } }
    public static PoolManager Pool { get { return Instance._poolManager; } }
    public static TimeManager Time { get { return Instance._timeManager; } }
    public static UIManager UI { get { return Instance._uiManager; } }
    public static SaveManager Save { get { return Instance._saveManager; } }
    public static UserData User { get { return Instance._saveManager.User; } } // 삭제대상
    public static GrowthSystem Growth { get { return Instance._growthSystem; } }
    public static PerkManager Perk { get { return Instance._perkManager; } }

    public static GameSession Session { get { return Instance._gameSession; } }
    public static ViewModelFactory ViewModel { get { return Instance._viewModelFactory; } }
    public static SoundManager Sound { get { return Instance._soundManager; } }


    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private SaveManager _saveManager = new();
    private GrowthSystem _growthSystem = new();
    private SoundManager _soundManager = new();
    private PerkManager _perkManager = new();

    private GameSession _gameSession;
    private ViewModelFactory _viewModelFactory;


    #endregion

    #region Variables

    private bool _initComplete = false;

    private LobbyController _realLobbyController;
    private LobbyController _dreamLobbyController;

    #endregion

    #region Init

    protected override void Init()
    {
        base.Init();

        _saveManager.Load();
        _dataTable.LoadAllData();

        // TODO: ui, network init

        InitializeAsync().Forget();
    }

    // TODO: 모바일은 OnApplicationPause(true)에서도 저장 필요
    private void OnApplicationQuit()
    {
        _saveManager.Save();
    }

    private async UniTask InitializeAsync()
    {
        // TODO: 네트워크로 부터 데이터를 받은 뒤 생성 - 비동기 await

        _gameSession = new();
        // TODO: 네트워크 매니저로 부터 데이터 요청 awit, 이 때 네트워크 매니저 주입

        // TODO 네트워크 매니저의 서비스 로직들 초기화

        _viewModelFactory = new(Session, DataTable);

        await _uiManager.Init();

        await _resourceManager.LoadContentAsync(AddressablePath.Label.COMMON);
        await _resourceManager.LoadContentAsync(AddressablePath.Label.REALITY);
        await _resourceManager.LoadContentAsync(AddressablePath.Label.DREAM);

        _soundManager.Init(this.gameObject);

        var poolRoot = Utils.CreateEmptyGameObject("PoolRoot", this.gameObject.transform).transform;
        await _poolManager.InitAsync(poolRoot);

        _initComplete = true;

        AutoWorkQueue.RunCollectLoopAsync(destroyCancellationToken).Forget();
        EnergyRecovery.RunRecoverLoopAsync(destroyCancellationToken).Forget();

        EnterReal();
        Sound.PlayBGM(AddressablePath.Audio.BGM_LOBBY);
    }

    #endregion

    public void EnterReal()
    {
        if(_realLobbyController == null)
        {
            _realLobbyController = new();
        }

        GameObject backgroundPrefab = Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.REAL_LOBBY_BACKGROUND);
        _realLobbyController.Enter(backgroundPrefab);
        UI.OpenRealHud();
    }

    public void ExitReal()
    {
        _realLobbyController.Release();
        UI.CloseRealHud();
    }

    public void EnterDream()
    {
        if (_dreamLobbyController == null)
        {
            _dreamLobbyController = new();
        }

        GameObject backgroundPrefab = Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.DREAM_LOBBY_BACKGROUND);
        _dreamLobbyController.Enter(backgroundPrefab);
        UI.OpenDreamHud();
    }

    public void ExitDream()
    {
        _dreamLobbyController.Release();
        UI.CloseDreamHud();
    }
}
