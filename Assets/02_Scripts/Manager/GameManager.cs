using Cysharp.Threading.Tasks;

public class GameManager : SingletonBehaviour<GameManager>
{
    public static ResourceManager Resource { get { return Instance._resourceManager; } }
    public static NetworkManager Network { get { return Instance._networkManager; } }
    public static DataTableManager DataTable { get { return Instance._dataTable; } }
    public static PoolManager Pool { get { return Instance._poolManager; } }
    public static TimeManager Time { get { return Instance._timeManager; } }
    public static UIManager UI { get { return Instance._uiManager; } }
    public static ViewModelManager ViewModel { get { return Instance._viewModelManager; } }

    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private ViewModelManager _viewModelManager = new();

    #endregion

    #region Variables

    private bool _initComplete = false;


    #endregion

    #region Init

    protected override void Init()
    {
        base.Init();

        _dataTable.LoadAllData();

        // TODO: ui, network init

        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        await _uiManager.Init();

        // TODO: 로딩 UI 연결
        await _resourceManager.PreloadAssetsAsync();

        var poolRoot = Utils.CreateEmptyGameObject("PoolRoot", this.gameObject.transform).transform;
        await _poolManager.InitAsync(poolRoot);

        _initComplete = true;

        // TODO: 로비가 생기면 시작 화면을 로비로 교체
        await _uiManager.OpenWorkInfoUI();
    }

    #endregion
}
