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
    public static ViewModelManager ViewModel { get { return Instance._viewModelManager; } }
    public static SaveManager Save { get { return Instance._saveManager; } }
    public static UserData User { get { return Instance._saveManager.User; } }
    public static GrowthSystem Growth { get { return Instance._growthSystem; } }
    public static CompanionManager Companion { get { return Instance._companionManager; } }
    public static EquipmentManager Equipment { get { return Instance._equipmentManager; } }
    public static PerkManager Perk { get { return Instance._perkManager; } }



    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private ViewModelManager _viewModelManager = new();
    private SaveManager _saveManager = new();
    private GrowthSystem _growthSystem = new();
    private CompanionManager _companionManager = new();
    private EquipmentManager _equipmentManager = new();
    private PerkManager _perkManager = new();


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
        await _uiManager.Init();

        await _resourceManager.LoadContentAsync(AddressablePath.Label.Common);
        await _resourceManager.LoadContentAsync(AddressablePath.Label.Reality);
        await _resourceManager.LoadContentAsync(AddressablePath.Label.Dream);


        var poolRoot = Utils.CreateEmptyGameObject("PoolRoot", this.gameObject.transform).transform;
        await _poolManager.InitAsync(poolRoot);

        _initComplete = true;

        AutoWorkQueue.RunCollectLoopAsync(destroyCancellationToken).Forget();
        EnergyRecovery.RunRecoverLoopAsync(destroyCancellationToken).Forget();

        EnterReal();
        await UI.OpenPerkInfoUI();
    }

    #endregion

    public void EnterReal()
    {
        if(_realLobbyController == null)
        {
            _realLobbyController = new();
        }

        GameObject backgroundPrefab = Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.RealLobbyBackground);
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

        GameObject backgroundPrefab = Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.DreamLobbyBackground);
        _dreamLobbyController.Enter(backgroundPrefab);
        UI.OpenDreamHud();
    }

    public void ExitDream()
    {
        _dreamLobbyController.Release();
        UI.CloseDreamHud();
    }
}
