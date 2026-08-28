using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public static ResourceManager Resource { get { return Instance._resourceManager; } }
    public static NetworkManager Network { get { return Instance._networkManager; } }
    public static DataTableManager DataTable { get { return Instance._dataTable; } }
    public static PoolManager Pool { get { return Instance._poolManager; } }
    public static TimeManager Time { get { return Instance._timeManager; } }
    public static UIManager UI { get { return Instance._uiManager; } }
    public static PerkManager Perk { get { return Instance._perkManager; } }
    public static SoundManager Sound { get { return Instance._soundManager; } }
    public static BattleManager Battle { get { return Instance._battleManager; } }

    public static GameSession Session { get { return Instance._gameSession; } }
    public static ViewModelFactory ViewModel { get { return Instance._viewModelFactory; } }



    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private SoundManager _soundManager = new();
    private PerkManager _perkManager = new();
    private BattleManager _battleManager = new();

    private GameSession _gameSession;
    private ViewModelFactory _viewModelFactory;


    #endregion

    private LobbyController _realLobbyController;
    private LobbyController _dreamLobbyController;

    private void Update()
    {
        Time.OnUpdate();
    }

    #region Init

    protected override void Init()
    {
        base.Init();

        _dataTable.LoadAllData();

        InitializeLoginAsync().Forget();
    }

    private async UniTask InitializeLoginAsync()
    {
        await _resourceManager.LoadContentAsync(AddressablePath.Label.LOGIN);
        await _uiManager.Init();

        _soundManager.Init(gameObject);
        _uiManager.OpenLoginUI();
    }

    public async UniTask InitializeAfterLoginAsync(Action<float> onProgress)
    {
        onProgress?.Invoke(0f);

        onProgress?.Invoke(0.15f);

        await _resourceManager.LoadAllLabelAssetAsync(
            progress => 
            { 
                onProgress?.Invoke(0.15f + progress * 0.65f); 
            });

        GameSession gameSession = new(_networkManager);
        await gameSession.LoadAllData();
        _gameSession = gameSession;

        _viewModelFactory = new(_gameSession, _dataTable);

        onProgress?.Invoke(0.85f);

        Transform poolRoot = Utils.CreateEmptyGameObject("PoolRoot",transform).transform;
        await _poolManager.InitAsync(poolRoot);

        AutoWorkQueue.RunCollectLoopAsync(destroyCancellationToken).Forget();
        EnergyRecovery.RunRecoverLoopAsync(destroyCancellationToken).Forget();

        onProgress?.Invoke(1f);

        EnterReal();
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
        Sound.PlayBGM(AddressablePath.Audio.BGM_LOBBY);
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

    public void RequestQuit()
    {
        QuitAfterSaveAsync().Forget();
    }

    private async UniTask QuitAfterSaveAsync()
    {
        await SaveUtil.SaveAllDataAsync();
        Application.Quit();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == false || _gameSession == null)
        {
            return;
        }

        SaveUtil.SaveAllDataAsync().Forget();
        AwayReportFlow.SetAppActive(!pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        AwayReportFlow.SetAppActive(hasFocus);
    }
}
