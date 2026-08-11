using Cysharp.Threading.Tasks;
using System;
using System.Threading;

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

    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private ViewModelManager _viewModelManager = new();
    private SaveManager _saveManager = new();

    #endregion

    #region Variables

    private const float AUTO_WORK_COLLECT_INTERVAL = 1f;

    private bool _initComplete = false;


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

        // TODO: 로딩 UI 연결
        await _resourceManager.PreloadAssetsAsync();

        var poolRoot = Utils.CreateEmptyGameObject("PoolRoot", this.gameObject.transform).transform;
        await _poolManager.InitAsync(poolRoot);

        _initComplete = true;

        CollectAutoWorkLoopAsync().Forget();

        // TODO: 로딩/로비가 생기면 지우기
        await _uiManager.OpenWorkInfoUI();
    }

    // 업무 화면이 닫혀 있어도 정산되도록 하기
    private async UniTaskVoid CollectAutoWorkLoopAsync()
    {
        CancellationToken token = destroyCancellationToken;

        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(AUTO_WORK_COLLECT_INTERVAL), ignoreTimeScale: true, cancellationToken: token);

            AutoWorkQueue.CollectCompleted();
        }
    }

    #endregion
}
